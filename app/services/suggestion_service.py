import ollama

class SuggestionService:
    async def suggest(
        self,
        source_text: str,
        source_language: str,
        target_language: str,
        context: str | None = None,
    ) -> str:
        
        system_instruction = (
            "You are an expert localization engineer. Translate the given text strictly respecting the context provided. "
            "Output ONLY the translated text without any explanation or quotes."
        )

        user_prompt = f"Source Language: {source_language}\nTarget Language: {target_language}\nContext/Key: {context or 'None'}\nSource Text: {source_text}\n\nTranslation:"

        try:
           
            response = ollama.chat(
                model='qwen2.5:7b',
                messages=[
                    {'role': 'system', 'content': system_instruction},
                    {'role': 'user', 'content': user_prompt}
                ]
            )
            
            result = response['message']['content'].strip()
            return result or f"[AI] {source_text}"

        except Exception as ex:
            print(f"[Ollama Error]: {ex}")
            return f"[AI Error] {source_text}"