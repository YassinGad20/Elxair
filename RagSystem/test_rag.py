from rag_chain import RAGChain

rag = RAGChain()

while True:

    question = input("You: ")

    if question.lower() == "exit":
        break

    response = rag.ask(question)

    print("\nAssistant:")
    print(response["answer"])
    print("-" * 70)