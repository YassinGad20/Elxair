from fastapi import FastAPI
from pydantic import BaseModel
import pandas as pd
import joblib
from datetime import datetime

app = FastAPI(
    title = "Elixir AI API",
    version = "1.0" 
)

print("THIS IS THE NEW API")

pipeline =  joblib.load("Random_Forest_Pipeline.pkl")

class PredictionItem(BaseModel):
    PerfumeId: int
    PerfumeSizeId: int
    PerfumeName: str
    Month: str
    BottleSize: str
    BottleVolume: int
    Category: str
    Gender: str
    Season: str
    SoldInSeason: int
    PriceCategory: str
    AvgMarginProfit: float
    AvgDiscount: float
    NumberOfTransactions: int

class PredictionReportRequest(BaseModel):
    PredictionMonth: int
    PredictionYear: int

    Items: list[PredictionItem]

class PredictionResult(BaseModel):
    PerfumeId: int
    PerfumeSizeId: int
    PerfumeName: str

    PredictedQuantity: int

class PredictionReport(BaseModel):
    GeneratedAt: datetime

    ModelVersion: str

    PredictionMonth: int

    PredictionYear: int

    TotalProducts: int

    Predictions: list[PredictionResult]



    

@app.post("/predict/report")
def predict(request: PredictionReportRequest):

    rows = []
    
    for item in request.Items:
        rows.append({
        "Month": item.Month,
        "BottleSize": item.BottleSize,
        "BottleVolume": item.BottleVolume,
        "Category": item.Category,
        "Gender": item.Gender,
        "Season": item.Season,
        "SoldInSeason": item.SoldInSeason,
        "PriceCategory": item.PriceCategory,
        "AvgMarginProfit": item.AvgMarginProfit,
        "AvgDiscount": item.AvgDiscount,
        "NumberOfTransactions": item.NumberOfTransactions
        })
    
    df = pd.DataFrame(rows)
    predictions = pipeline.predict(df)

    results = []

    for item, pred in zip(request.Items, predictions):
        results.append(
            PredictionResult(
                PerfumeId=item.PerfumeId,
                PerfumeSizeId=item.PerfumeSizeId,
                PerfumeName=item.PerfumeName,
                PredictedQuantity=int(round(pred))
            )
        )

    return PredictionReport(
    GeneratedAt=datetime.now(),
    ModelVersion="RandomForest_v1",
    PredictionMonth=request.PredictionMonth,
    PredictionYear=request.PredictionYear,
    TotalProducts=len(results),
    Predictions=results
)

