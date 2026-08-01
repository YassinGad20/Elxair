from retriever import Retriever
from llm import LLMService
from prompt_builder import PromptBuilder
from system_prompt import SYSTEM_PROMPT
from memory import ConversationMemory


class RAGChain:

    def __init__(self):

        self.retriever = Retriever()
        self.llm = LLMService()
        self.memory = ConversationMemory()

    def ask(self, question: str):

        # 1) Retrieve relevant documents
        documents = self.retriever.retrieve(question)

        # 2) Build user prompt
        user_prompt = PromptBuilder.build(
            question=question,
            documents=documents
        )

        # 3) Get conversation history
        history = self.memory.get_messages()

        # 4) Generate answer
        answer = self.llm.generate(
            system_prompt=SYSTEM_PROMPT,
            history=history,
            user_prompt=user_prompt
        )

        # 5) Save conversation
        self.memory.add_user_message(question)
        self.memory.add_ai_message(answer)

        # 6) Return answer + retrieved sources
        return {
            "answer": answer,
            "documents": documents
        }

    def clear_memory(self):

        self.memory.clear()