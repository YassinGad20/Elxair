from openai import OpenAI
from dotenv import load_dotenv
import os

load_dotenv()


class LLMService:

    def __init__(self):

        self.client = OpenAI(
            base_url=os.getenv("LM_STUDIO_BASE_URL"),
            api_key="lm-studio"
        )

        self.model = os.getenv("CHAT_MODEL")

    def generate(self,system_prompt , history ,  user_prompt):

       
            
            messages=[
                {
                    "role": "system",
                    "content": system_prompt
                },
                *history,
                {
                    "role": "user",
                    "content": user_prompt
                }
            ]
            response = self.client.chat.completions.create(
                model=self.model,
                messages=messages,
                temperature=0.2
            )
        
            return response.choices[0].message.content


    def rewrite_question(self, question, history):

                messages = [
                    {
                        "role": "system",
                        "content": """
            You are a query rewriting assistant.

            Your task is to rewrite the user's latest question into a complete standalone question using the conversation history.

            Rules:
            - Do NOT answer the question.
            - Do NOT explain.
            - Return ONLY the rewritten question.
            - If the question is already complete, return it unchanged.
            """
                    },

                    *history,

                    {
                        "role": "user",
                        "content": question
                    }
                ]

                response = self.client.chat.completions.create(
                    model=self.model,
                    messages=messages,
                    temperature=0
                )

                return response.choices[0].message.content.strip()