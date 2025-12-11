import os
from typing import List, Dict
from langchain_community.document_loaders import DirectoryLoader, TextLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_community.embeddings import HuggingFaceEmbeddings
from langchain_community.vectorstores import FAISS
from langchain_openai import ChatOpenAI
from langchain.prompts import ChatPromptTemplate
from dotenv import load_dotenv

load_dotenv()

class RagEngine:
    def __init__(self, docs_path: str):
        self.docs_path = docs_path
        self.vector_store = None
        self.llm = ChatOpenAI(model_name="gpt-3.5-turbo", temperature=0)
        self.embeddings = HuggingFaceEmbeddings(model_name="all-MiniLM-L6-v2")
        
        # Initialize knowledge base
        self.load_knowledge_base()

    def load_knowledge_base(self):
        print("Cargando base de conocimientos...")
        loader = DirectoryLoader(self.docs_path, glob="**/*.md", loader_cls=TextLoader)
        documents = loader.load()
        
        text_splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=50)
        texts = text_splitter.split_documents(documents)
        
        self.vector_store = FAISS.from_documents(texts, self.embeddings)
        print("Base de conocimientos cargada.")

    def query(self, question: str, role: str = "User") -> Dict:
        # 1. Retrieve relevant docs
        docs = self.vector_store.similarity_search(question, k=3)
        context = "\n\n".join([d.page_content for d in docs])
        
        # 2. Construct Prompt
        system_prompt = f"""
        Eres un asistente experto para el sistema TurnosMedicos.
        Tu rol es ayudar a usuarios con rol: {role}.
        
        Usa el siguiente contexto para responder a la pregunta del usuario.
        Si la pregunta requiere una consulta a la base de datos, genera una consulta SQL sugerida basada en los ejemplos del contexto.
        
        Contexto:
        {context}
        
        Formato de respuesta (JSON):
        {{
            "respuesta": "Explicación en texto...",
            "sql_sugerido": "SELECT ... (o null si no aplica)",
            "notas": "Advertencias o comentarios extra"
        }}
        """
        
        prompt = ChatPromptTemplate.from_messages([
            ("system", system_prompt),
            ("user", "{question}")
        ])
        
        # 3. Call LLM
        chain = prompt | self.llm
        response = chain.invoke({"question": question})
        
        # Note: In a real scenario, we would parse the JSON. 
        # For now, we return the raw content, assuming the LLM follows instructions or we parse it.
        # To ensure JSON, we could use structured output parsers.
        
        return {
            "raw_response": response.content
        }
