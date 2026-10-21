
import unittest
import numpy as np
import pandas as pd

from config.config import FEATURE_COLUMNS
from preprocessing.feature_engineering import build_features


class TestFeatureEngineering(unittest.TestCase):

    def test_feature_count(self):
        times = pd.date_range(
            "2026-01-01",
            periods=216,
            freq="h",
        )

        n = len(times)

        raw_df = pd.DataFrame({
            "datetime": times,
            "Temperature": np.full(n, 25.0),
            "Relative_Humidity": np.full(n, 80.0),
            "Surface_Pressure": np.full(n, 1010.0),
            "Precipitation": np.zeros(n),
            "Cloud_Cover_Percent": np.full(n, 50.0),
            "Wind_Speed_kmh": np.full(n, 10.0),
            "Irradiance_Surface": np.zeros(n),
        })

        features = build_features(raw_df)

        self.assertEqual(len(FEATURE_COLUMNS), 54)
        self.assertEqual(features.shape[1], 55)
        # 55 cột = datetime + 54 features
        self.assertGreaterEqual(len(features), 24)

    def test_insufficient_history(self):
        times = pd.date_range(
            "2026-01-01",
            periods=191,
            freq="h",
        )

        n = len(times)

        raw_df = pd.DataFrame({
            "datetime": times,
            "Temperature": np.full(n, 25.0),
            "Relative_Humidity": np.full(n, 80.0),
            "Surface_Pressure": np.full(n, 1010.0),
            "Precipitation": np.zeros(n),
            "Cloud_Cover_Percent": np.full(n, 50.0),
            "Wind_Speed_kmh": np.full(n, 10.0),
            "Irradiance_Surface": np.zeros(n),
        })

        with self.assertRaisesRegex(
            ValueError,
            "cần tối thiểu 24 dòng",
        ):
            build_features(raw_df)


if __name__ == "__main__":
    unittest.main()
