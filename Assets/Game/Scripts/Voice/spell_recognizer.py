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
    "ледяная стрела": "ICE_ARROW"
}

model = Model(MODEL_PATH)
rec = KaldiRecognizer(model, 16000)
q = queue.Queue()

def callback(indata, frames, time, status):
    q.put(bytes(indata))

with sd.RawInputStream(
        samplerate=16000,
        blocksize=8000,
        dtype='int16',
        channels=1,
        callback=callback):

    print("READY")
    while True:
        data = q.get()
        if rec.AcceptWaveform(data):
            result = json.loads(rec.Result())
            text = result.get("text", "").lower()

            for spell in SPELLS:
                if spell in text:
                    print(SPELLS[spell], flush=True)
