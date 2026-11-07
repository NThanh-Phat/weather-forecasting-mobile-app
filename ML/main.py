from datetime import datetime
from typing import List

import pandas as pd
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field

from config.config import TIMEZONE, FORECAST_HOURS
from inference.predictor import predict_next_24_hours

app = FastAPI(
    title="Weather Forecasting API", 
    description="API dự báo nhiệt độ 24 giờ bằng mô hình GRU",
    version="1.0.0")


#Request model

class ForecastRequest(BaseModel):
    latitude: float = Field(ge=-90, le=90)
    longitude: float = Field(ge=-180, le=180)

#Response

class PredictionItem(BaseModel):
    forecastTime: datetime
    predictedTemperature: float

class ForecastResponse(BaseModel):
    success: bool
    timezone: str   
    forecastHours: int
    predictions: List[PredictionItem]

#Health Check Endpoint

@app.get("/health")
def health_check():
    return {"status": "healthy",
            "service": "Weather Forecasting API",
            }


#Forecast Endpoint

@app.post("/api/predict", response_model=ForecastResponse)
def predict(request: ForecastRequest):
    try:
        result = predict_next_24_hours(
            request.latitude, 
            request.longitude
            )

        if result is None or len(result) != FORECAST_HOURS:
            raise ValueError(f"GRU phải trả về đúng {FORECAST_HOURS} kết quả.")

        if result [["datetime","Predicted_Temperature_C"]].isnull().any().any():
            raise ValueError("Kết quả dự báo chứa dữ liệu thiếu.")   
        predictions = []

        for _, row in result.iterrows():
            predictions.append(
                PredictionItem(
                    forecastTime=pd.Timestamp(row["datetime"]).to_pydatetime(),
                    predictedTemperature=float(
                        row["Predicted_Temperature_C"]
                    )
                )
            )

        return ForecastResponse(
            success=True,
            timezone=TIMEZONE,
            forecastHours=FORECAST_HOURS,
            predictions=predictions
        )

    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Dự báo thất bại: {str(e)}"
        )
