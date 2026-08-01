from fastapi import FastAPI
from rag_chain import RAGChain
from models import ChatRequest , ChatResponse

app = FastAPI(
    title="Elixir Chatbot"
)

rag = RAGChain()

@app.post("/chat" , response_model=ChatResponse)
def chat(request: ChatRequest):

    result = rag.ask(request.message)

    return ChatResponse(
        answer=result["answer"]
    )

@app.post("/clear-memory")
def clear_memory():

    rag.clear_memory()

    return {
        "message":"Conversation memory cleared."
    }