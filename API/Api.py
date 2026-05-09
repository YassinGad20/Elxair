from fastapi import FastAPI
from pydantic import BaseModel, Field
import pandas as pd
import joblib

# ================== LOAD ==================
model = joblib.load('Random Forest.pkl')
ohe = joblib.load('ohe_encoder.pkl')
scaler = joblib.load('scaler_columns.pkl')
scaler_cols = joblib.load('scaler_num_cols.pkl')
te = joblib.load('target_encoder.pkl')

FEATURE_COLUMNS = [
    'Size', 'Sold In Season', 'Perfume Demand', 'Unit Price',
    'Gender_Female', 'Gender_Male', 'Gender_Unisex',
    'Category_Aquatic', 'Category_Aromatic', 'Category_Floral', 'Category_Fresh',
    'Category_Gourmand', 'Category_Musk', 'Category_Oriental', 'Category_Oud',
    'Category_Spicy', 'Category_Woody',
    'Perfume Season_Spring/Summer', 'Perfume Season_Winter'
]

SIZE_MAPPING = {
    '3ml': 0, '5ml': 1, '10ml': 2, '15ml': 3,
    '30ml': 4, '50ml': 5, '100ml': 6
}

app = FastAPI()

class DataModel(BaseModel):
    Size: str
    Sold_In_Season: str = Field(alias="Sold In Season")
    Perfume_Demand: int = Field(alias="Perfume Demand")
    Unit_Price: float = Field(alias="Unit Price") # تغيير من int لـ float
    Gender: str
    Category: str
    # جعلنا Perfume Season اختياري عشان الـ C# مش بيبعته حالياً
    Perfume_Season: str = Field(default="Winter", alias="Perfume Season") 

    class Config:
        populate_by_name = True

@app.post("/predict") # تغيير المسار ليتوافق مع C#
def predict(item: DataModel):
    try:
        data = item.model_dump(by_alias=True)
        df = pd.DataFrame([data])

        # 1. Size mapping
        df['Size'] = df['Size'].map(SIZE_MAPPING).fillna(0)

        # 2. Target Encoding
        df['Sold In Season'] = te.transform(df['Sold In Season'])

        # 3. OHE
        cat_cols = ['Gender', 'Category', 'Perfume Season']
        # تأكد أن القيم موجودة ولا تسبب خطأ في OHE
        encoded = ohe.transform(df[cat_cols])
        encoded_df = pd.DataFrame(
            encoded,
            columns=ohe.get_feature_names_out(cat_cols),
            index=df.index
        )
        df = df.drop(columns=cat_cols)
        df = pd.concat([df, encoded_df], axis=1)

        # 4. ترتيب الأعمدة
        df = df.reindex(columns=FEATURE_COLUMNS, fill_value=0)

        # 5. Scaling
        df[scaler_cols] = scaler.transform(df[scaler_cols])

        # 6. Predict
        yhat = model.predict(df[FEATURE_COLUMNS].values)

        # الرد بالاسم اللي C# مستنيه
        return {"PredictedProfit": float(yhat[0])}

    except Exception as e:
        return {"error": str(e)}