from document_loader import ReportDocumentLoader
from vector_store import VectorStoreService


loader = ReportDocumentLoader(
    r"D:\Fci 2025-2026\Software Development\Elxair_Project\Elxair\Elxair\wwwroot\Reports\Json"
)

documents = loader.load_report(7, 2026)

vector_db = VectorStoreService()

vector_db.add_documents(documents)

print(f"Indexed {len(documents)} documents.")