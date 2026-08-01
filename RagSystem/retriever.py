from vector_store import VectorStoreService


class Retriever:

    def __init__(self):

        self.vector_db = VectorStoreService()
    
    
    def retrieve(self, question, k=3):

        return self.vector_db.similarity_search(
            query=question,
            k=k
        )