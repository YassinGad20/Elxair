from langchain_chroma import Chroma
from embedder import LMStudioEmbeddings

class VectorStoreService:

    def __init__(self):

        self.embedding = LMStudioEmbeddings()
        self.vector_store = Chroma(
            collection_name = "business_reports",
            embedding_function = self.embedding,
            persist_directory = "vector_db"
        )


    def add_documents(self,documents):

        self.vector_store.add_documents(documents)

    def similarity_search(self , query , k = 3):
        return self.vector_store.similarity_search(
            query=query,
            k=k
        )

    def get_vector_store(self):

        return self.vector_store