from fastapi import APIRouter

from app.schemas.suggestion import (
    SuggestionRequest,
    SuggestionResponse,
    BatchSuggestionRequest,
    BatchSuggestionResponse
)

from app.services.suggestion_service import SuggestionService


router = APIRouter(
    prefix="/api/review",
    tags=["AI Suggestion"],
)

service = SuggestionService()

#Dich 1 tu
@router.post(
    "/suggest",
    response_model=SuggestionResponse,
)
async def suggest_translation(
    request: SuggestionRequest,
) -> SuggestionResponse:

    suggestion = await service.suggest(
        source_text=request.source_text,
        source_language=request.source_language,
        target_language=request.target_language,
        context=request.context,
    )

    return SuggestionResponse(
        suggestion=suggestion
    )

# Translate json
@router.post(
    "/suggest-batch",
    response_model=BatchSuggestionResponse,
)
async def suggest_translation_batch(
    request: BatchSuggestionRequest,
) -> BatchSuggestionResponse:
    
    # Gọi hàm suggest_batch từ SuggestionService
    result_dict = await service.suggest_batch(
        json_data=request.data,
        source_language=request.source_language,
        target_language=request.target_language
    )

    return BatchSuggestionResponse(
        suggestions=result_dict
    )