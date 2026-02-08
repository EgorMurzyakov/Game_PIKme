import json
import queue
import sounddevice as sd
import os
from vosk import Model, KaldiRecognizer

MODEL_PATH = os.path.join(os.path.dirname(__file__), "vosk-model-small-ru-0.22")


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
