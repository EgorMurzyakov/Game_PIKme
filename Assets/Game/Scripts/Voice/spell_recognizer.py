import json
import queue
import sounddevice as sd
import os
import sys
from vosk import Model, KaldiRecognizer


def resource_path(rel_path: str) -> str:
    # Если запущено из PyInstaller
    if getattr(sys, "frozen", False) and hasattr(sys, "_MEIPASS"):
        base = sys._MEIPASS
    else:
        base = os.path.dirname(os.path.abspath(__file__))
    return os.path.join(base, rel_path)

MODEL_PATH = resource_path("vosk-model-small-ru-0.22")


SPELLS = {
    "огненный шар": "FIREBALL",
    "торнадо": "TORNADO",
    "ледяная стрела": "ICE_ARROW",
    "гойда": "GOYDA",
    "господи помилуй": "GOD_HELP",
    "отче наш": "GOD_HELP"
}

model = Model(MODEL_PATH)
rec = KaldiRecognizer(model, 16000)
q = queue.Queue()

def callback(indata, frames, time, status):
    q.put(bytes(indata))

with sd.RawInputStream(
        samplerate=16000,
        blocksize=4000,
        dtype='int16',
        channels=1,
        callback=callback):

    print("READY (Ctrl+C to stop)")
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
        print("Stopping...")
