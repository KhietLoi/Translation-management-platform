from typing import Dict
from pydantic import BaseModel

class SuggestionRequest(BaseModel):
    source_text: str
    source_language: str
    target_language: str
    context: str | None = None


class SuggestionResponse(BaseModel):
    suggestion: str

class BatchSuggestionRequest(BaseModel):
    source_language: str
    target_language: str
    data: Dict[str, str]  # Nhận cục JSON (vd: {"id1": "Đăng nhập", "id2": "Hủy"})

class BatchSuggestionResponse(BaseModel):
    suggestions: Dict[str, str] # Trả về cục JSON đã dịch