from pathlib import Path
import json
from langchain_core.documents import Document


class ReportDocumentLoader:

    def __init__(self,reports_folder:str):
        self.reports_folder = Path(
    r"D:\Fci 2025-2026\Software Development\Elxair_Project\Elxair\Elxair\wwwroot\Reports\Json"
)

    def load_report(self, month:int , year:int):
        report_path = self.reports_folder / f"Report_{year}_{month:02d}.json"

        if not report_path.exists():
            raise FileNotFoundError(
                f"Report not found: {report_path}"
            )

        with open(report_path , "r" , encoding="utf-8") as file:
            report = json.load(file)

        return self._build_documents(report)

    def _build_documents(self , report):

        documents = []

        month = report["ReportMonth"]
        year = report["ReportYear"]

        # Executive Summary
        
        summary = f"""

    Business Report

    Month: {month}
    Year: {year}

    Total Predicted Profit: {report["TotalPredictedProfit"]}
    Total Predicted Units: {report["TotalPredictedUnits"]}
    Total Products: {report["TotalProductsCount"]}
"""

        top = report["TopPerformer"]

        documents.append(
            Document(
                page_content=f"""
        Top Performer

        Perfume: {top["PerfumeName"]}

        Category: {top["CategoryName"]}

        Gender: {top["Gender"]}

        Size: {top["Size"]}

        Predicted Quantity: {top["PredictedQuantity"]}

        Predicted Profit: {top["PredictedProfit"]}
        """.strip(),

                        metadata={
                            "type": "top_performer",
                            "month": month,
                            "year": year,
                            "perfume": top["PerfumeName"]
                        }
                    )
                )

        # -----------------------------
        # Lowest Performer
        # -----------------------------

        lowest = report["LowestPerformer"]

        documents.append(
            Document(
                page_content=f"""
        Lowest Performer

        Perfume: {lowest["PerfumeName"]}

        Category: {lowest["CategoryName"]}

        Gender: {lowest["Gender"]}

        Size: {lowest["Size"]}

        Predicted Quantity: {lowest["PredictedQuantity"]}

        Predicted Profit: {lowest["PredictedProfit"]}
        """.strip(),

                metadata={
                    "type": "lowest_performer",
                    "month": month,
                    "year": year,
                    "perfume": lowest["PerfumeName"]
                }
            )
        )

        for product in report["Top10Products"]:

            documents.append(
                Document(
                    page_content=f"""
        Product

        Perfume: {product["PerfumeName"]}

        Category: {product["CategoryName"]}

        Gender: {product["Gender"]}

        Size: {product["Size"]}

        Predicted Quantity: {product["PredictedQuantity"]}

        Predicted Profit: {product["PredictedProfit"]}
        """.strip(),

                            metadata={
                                "type": "product",
                                "month": month,
                                "year": year,
                                "perfume": product["PerfumeName"],
                                "size": product["Size"]
                            }
                        )
                    )

        # -----------------------------
        # Categories
        # -----------------------------

        for category in report["Categories"]:

            documents.append(
                Document(
                    page_content=f"""
        Category

        Name: {category["CategoryName"]}

        Total Units: {category["TotalUnits"]}

        Total Profit: {category["TotalProfit"]}
        """.strip(),

                    metadata={
                        "type": "category",
                        "month": month,
                        "year": year,
                        "category": category["CategoryName"]
                    }
                )
            )

        # -----------------------------
        # Genders
        # -----------------------------

        for gender in report["Genders"]:

            documents.append(
                Document(
                    page_content=f"""
        Gender

        Name: {gender["Gender"]}

        Total Units: {gender["TotalUnits"]}

        Total Profit: {gender["TotalProfit"]}
        """.strip(),

                    metadata={
                        "type": "gender",
                        "month": month,
                        "year": year,
                        "gender": gender["Gender"]
                    }
                )
            )

        # -----------------------------
        # Sizes
        # -----------------------------

        for size in report["Sizes"]:

            documents.append(
                Document(
                    page_content=f"""
        Size

        Name: {size["Size"]}

        Total Units: {size["TotalUnits"]}

        Total Profit: {size["TotalProfit"]}
        """.strip(),

                    metadata={
                        "type": "size",
                        "month": month,
                        "year": year,
                        "size": size["Size"]
                    }
                )
            )

        return documents