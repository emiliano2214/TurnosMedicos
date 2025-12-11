from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from rag_engine import RagEngine
import os

app = FastAPI(title="TurnosMedicos AI Service")

# Initialize RAG Engine
# Assuming docs are in ../docs relative to this file
DOCS_PATH = os.path.join(os.path.dirname(__file__), "../docs")
rag_engine = RagEngine(DOCS_PATH)

class ChatRequest(BaseModel):
    rol: str
    pregunta: str
    historial: list = []

@app.post("/chat")
async def chat(request: ChatRequest):
    try:
        response = rag_engine.query(request.pregunta, request.rol)
        return response
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/health")
async def health():
    return {"status": "ok"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
