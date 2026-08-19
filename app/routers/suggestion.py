from fastapi import APIRouter

from app.schemas.suggestion import (
    SuggestionRequest,
    SuggestionResponse,
)

from app.services.suggestion_service import SuggestionService


router = APIRouter(
    prefix="/api/review",
    tags=["AI Suggestion"],
)

service = SuggestionService()


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