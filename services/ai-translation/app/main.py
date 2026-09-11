from fastapi import FastAPI

from app.routers.suggestion import router as suggestion_router


app = FastAPI(
    title="MySolution AI Service",
    version="1.0.0",
)


app.include_router(suggestion_router)


@app.get("/health")
async def health():
    return {
        "status": "healthy"
    }