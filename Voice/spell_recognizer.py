import json
import queue
import sounddevice as sd
import os
import sys
import argparse
from vosk import Model, KaldiRecognizer


def resource_path(rel_path: str) -> str:
    if getattr(sys, "frozen", False):
        base = getattr(sys, "_MEIPASS", os.path.dirname(sys.executable))
    else:
        base = os.path.dirname(os.path.abspath(__file__))
    return os.path.join(base, rel_path)


MODEL_PATH = resource_path("vosk-model-small-ru-0.22")

SPELLS = {
    "огненный шар": "FIREBALL",
    "торнадо": "TORNADO",
}


def list_microphones():
    devices = sd.query_devices()
    unique_mics = []

    # Системные заглушки, которые не являются реальными микрофонами
    IGNORE = {"input ()", "микрофон ()", "первичный драйвер записи звука", 
              "переназначение звуковых устр. - input"}

    for i, dev in enumerate(devices):
        if dev["max_input_channels"] <= 0:
            continue
            
        name = dev["name"].strip()
        if not name or name.lower() in IGNORE:
            continue

        is_duplicate = False
        # Нормализуем для сравнения (убираем регистр и лишние пробелы)
        norm_new = name.lower().strip()

        for idx, (dev_id, dev_name) in enumerate(unique_mics):
            norm_exist = dev_name.lower().strip()
            # Если одно название начинается с другого → это обрезанный дубликат PortAudio
            if norm_new.startswith(norm_exist) or norm_exist.startswith(norm_new):
                is_duplicate = True
                # Оставляем вариант с БОЛЕЕ ПОЛНЫМ названием
                if len(name) > len(dev_name):
                    unique_mics[idx] = (dev_id, name)
                break

        if not is_duplicate:
            unique_mics.append((i, name))

    return unique_mics


def pick_microphone_interactive():
    """Интерактивный выбор микрофона в консоли."""
    mics = list_microphones()
    if not mics:
        print("Нет доступных микрофонов!", file=sys.stderr)
        sys.exit(1)

    print("Доступные микрофоны:", file=sys.stderr)
    for idx, (dev_id, name) in enumerate(mics):
        marker = " <-- по умолчанию" if dev_id == sd.default.device[0] else ""
        print(f"  [{idx}] (ID={dev_id}) {name}{marker}", file=sys.stderr)

    print(f"\nВведите номер из списка (Enter = по умолчанию): ", end="", file=sys.stderr)
    choice = input().strip()

    if choice == "":
        return None  # системный дефолт

    try:
        chosen = mics[int(choice)]
        print(f"Выбран микрофон: {chosen[1]} (ID={chosen[0]})", file=sys.stderr)
        return chosen[0]
    except (ValueError, IndexError):
        print("Некорректный ввод, используется микрофон по умолчанию.", file=sys.stderr)
        return None


def parse_args():
    parser = argparse.ArgumentParser(description="Spell recognizer")
    group = parser.add_mutually_exclusive_group()
    group.add_argument(
        "--device", type=int, default=None,
        help="ID устройства ввода (см. --list)"
    )
    group.add_argument(
        "--list", action="store_true",
        help="Показать список микрофонов и выйти"
    )
    group.add_argument(
        "--pick", action="store_true",
        help="Интерактивно выбрать микрофон перед запуском"
    )
    return parser.parse_args()


def main():
    args = parse_args()

    # Просто вывести список и выйти
    if args.list:
        for dev_id, name in list_microphones():
            print(f"ID={dev_id}  {name}")
        sys.exit(0)

    # Определяем устройство
    if args.pick:
        device_id = pick_microphone_interactive()
    elif args.device is not None:
        device_id = args.device
        dev_info = sd.query_devices(device_id)
        print(f"Используется микрофон: {dev_info['name']} (ID={device_id})", file=sys.stderr)
    else:
        device_id = None  # системный дефолт

    model = Model(MODEL_PATH)
    rec = KaldiRecognizer(model, 16000)
    q = queue.Queue()

    def callback(indata, frames, time, status):
        q.put(bytes(indata))

    with sd.RawInputStream(
            samplerate=16000,
            blocksize=4000,
            dtype="int16",
            channels=1,
            device=device_id, 
            callback=callback):

        print("READY (Ctrl+C to stop)", file=sys.stderr)
        detected_spells = set()
        try:
            while True:
                try:
                    data = q.get(timeout=1.0)
                except queue.Empty:
                    continue

                if rec.AcceptWaveform(data):
                    result = json.loads(rec.Result())
                    text = result.get("text", "").lower()
                    final = True
                else:
                    result = json.loads(rec.PartialResult())
                    text = result.get("partial", "").lower()
                    final = False

                for spell in SPELLS:
                    if spell in text and spell not in detected_spells:
                        print(SPELLS[spell], flush=True)
                        detected_spells.add(spell)

                if final:
                    detected_spells.clear()

        except KeyboardInterrupt:
            print("Stopping...", file=sys.stderr)


if __name__ == "__main__":
    main()