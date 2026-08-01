from openai import OpenAI
from dotenv import load_dotenv
import os

load_dotenv()


class LMStudioEmbeddings:

    def __init__(self):
        self.client = OpenAI(
            base_url=os.getenv("LM_STUDIO_BASE_URL"),
            api_key="lm-studio"
        )

        self.model = os.getenv("EMBEDDING_MODEL")

    def embed_documents(self, texts: list[str]) -> list[list[float]]:

        response = self.client.embeddings.create(
            model=self.model,
            input=texts
        )

        return [item.embedding for item in response.data]

    def embed_query(self, text: str) -> list[float]:

        response = self.client.embeddings.create(
            model=self.model,
            input=text
        )

        return response.data[0].embedding