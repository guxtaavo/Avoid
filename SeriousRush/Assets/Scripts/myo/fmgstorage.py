import matplotlib
matplotlib.use('tkagg')
import numpy as np
import matplotlib.pyplot as plt
from scipy.interpolate import interp1d
import hyperion
import time
import pandas as pd
from collections import deque

# -------- Sample Rate Monitor --------
class SampleRateMonitor:
    def __init__(self, window_size=100):
        self.timestamps = deque(maxlen=window_size)

    def update(self):
        self.timestamps.append(time.time())

    def get_rate(self):
        if len(self.timestamps) < 2:
            return 0
        elapsed = self.timestamps[-1] - self.timestamps[0]
        return (len(self.timestamps) - 1) / elapsed if elapsed > 0 else 0

# -------- Setup --------
channel = 1
h1 = hyperion.Hyperion('10.0.0.55')

spectra_log = []
monitor = SampleRateMonitor()
start_time = time.time()
CALIBRATION_DURATION = 5  # seconds
fixed_sample_rate = None
storage_interval = None
last_stored_time = None

plt.ion()

while True:
    try:
        # Acquire spectrum
        timestamp = time.time()
        peaks = h1.peaks[channel]
        spectra = h1.spectra
        spectrum = spectra[channel]
        wavelengths = spectra.wavelengths
        peak_wl = wavelengths[np.argmax(spectrum)]
        print(f"Peak: {peak_wl:.2f}")

        # CALIBRATION
        if fixed_sample_rate is None:
            monitor.update()
            if timestamp - start_time >= CALIBRATION_DURATION:
                fixed_sample_rate = monitor.get_rate()
                storage_interval = 1.0 / fixed_sample_rate
                last_stored_time = timestamp
                print(f"Estimated max sample rate: {fixed_sample_rate:.2f} Hz")
            continue  # Skip logging and plotting during calibration

        # FIXED INTERVAL LOGGING
        if timestamp - last_stored_time >= storage_interval:
            spectra_log.append({
                "timestamp": timestamp,
                "peak_wavelength": peak_wl,
                **{f"wl_{i}": wl for i, wl in enumerate(wavelengths)},
                **{f"intensity_{i}": val for i, val in enumerate(spectrum)}
            })
            last_stored_time += storage_interval  # advance clock like metronome

            # Plot (only when logging a sample)
            plt.clf()
            plt.plot(wavelengths, spectrum, label='FMG - CH1')
            plt.legend()
            plt.pause(0.001)

    except KeyboardInterrupt:
        df = pd.DataFrame(spectra_log)
        df.to_csv("hyperion_spectra_log.csv", index=False)
        print("Spectral data saved.")
        plt.clf()
        exit()
