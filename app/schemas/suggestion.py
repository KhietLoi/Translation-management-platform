from pydantic import BaseModel


class SuggestionRequest(BaseModel):
    source_text: str
    source_language: str
    target_language: str
    context: str | None = None


class SuggestionResponse(BaseModel):
    suggestion: str