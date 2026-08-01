

class PromptBuilder:

    @staticmethod
    def build(question , documents ):

        context = "\n\n".join(
            doc.page_content for doc in documents
        )

        return f""" 
Business Report Context:
{context}

Question:
{question}

Answer:
"""
        