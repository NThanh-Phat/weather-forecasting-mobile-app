
import unittest
from unittest.mock import patch, Mock

from preprocessing.openmeteo_client import fetch_weather


class TestOpenMeteoClient(unittest.TestCase):

    @patch("preprocessing.openmeteo_client.requests.get")
    def test_fetch_weather(self, mock_get):
        response = Mock()

        response.raise_for_status.return_value = None

        response.json.return_value = {
            "hourly": {
                "time": [
                    "2026-01-01T00:00",
                    "2026-01-01T01:00",
                ],
                "temperature_2m": [25.0, 25.2],
                "relative_humidity_2m": [80, 79],
                "surface_pressure": [1010, 1010],
                "precipitation": [0, 0],
                "cloud_cover": [50, 45],
                "wind_speed_10m": [10, 12],
                "shortwave_radiation": [0, 10],
            }
        }

        mock_get.return_value = response

        df = fetch_weather()

        self.assertEqual(len(df), 2)
        self.assertIn("Temperature", df.columns)
        self.assertIn("Relative_Humidity", df.columns)
        self.assertIn("datetime", df.columns)


if __name__ == "__main__":
    unittest.main()
