import json
import time
import ollama


class SuggestionService:

    MODEL_NAME = "qwen2.5:1.5b"

    async def suggest(
        self,
        source_text: str,
        source_language: str,
        target_language: str,
        context: str | None = None
    ) -> str:

        try:
            response = ollama.chat(
                model=self.MODEL_NAME,
                messages=[
                    {
                        "role": "system",
                        "content": (
                            f"Translate {source_language} to {target_language}. "
                            "Return only the translation. "
                            "Preserve placeholders."
                        )
                    },
                    {
                        "role": "user",
                        "content": (
                            f"{source_text}\n"
                            "Translation:"
                        )
                    }
                ],
                options={
                    "temperature": 0.2,
                    "num_ctx": 512,
                    "num_predict": 128
                }
            )

            return response["message"]["content"].strip()

        except Exception as ex:
            print(f"[Single Error] {ex}")
            return f"[AI Error] {source_text}"

    async def suggest_batch(
        self,
        json_data: dict,
        source_language: str,
        target_language: str
    ) -> dict:

        if not json_data:
            return {}

        start = time.perf_counter()

        print(
            f"[Ollama Batch] "
            f"{len(json_data)} items | "
            f"{source_language}->{target_language}"
        )

        prompt = (
            f"Translate every value from {source_language} "
            f"to {target_language}.\n"
            "Keep every key exactly unchanged.\n"
            "Translate values only.\n"
            "Return one valid JSON object.\n"
            "Do not omit any key.\n"
            "Preserve placeholders.\n\n"
            f"{json.dumps(json_data, ensure_ascii=False)}"
        )

        try:
            response = ollama.chat(
                model=self.MODEL_NAME,
                messages=[
                    {
                        "role": "system",
                        "content": "Translate JSON. Return JSON only."
                    },
                    {
                        "role": "user",
                        "content": prompt
                    }
                ],
                format="json",
                options={
                    "temperature": 0,
                    "num_ctx": 4096,
                    "num_predict": 3072
                }
            )

            text = response["message"]["content"].strip()

            result = json.loads(text)

            if (
                isinstance(result, dict)
                and isinstance(result.get("translations"), dict)
            ):
                result = result["translations"]

            result = {
                key: result[key].strip()
                for key in json_data
                if key in result
                and isinstance(result[key], str)
            }

            missing = {
                key: value
                for key, value in json_data.items()
                if key not in result
            }

            if missing:
                print(
                    f"[Ollama Batch] Missing "
                    f"{len(missing)} keys, retrying..."
                )

                retry_prompt = (
                    f"Translate every value from {source_language} "
                    f"to {target_language}.\n"
                    "Keep the keys exactly unchanged.\n"
                    "Return JSON only.\n\n"
                    f"{json.dumps(missing, ensure_ascii=False)}"
                )

                retry_response = ollama.chat(
                    model=self.MODEL_NAME,
                    messages=[
                        {
                            "role": "system",
                            "content": "Translate JSON. Return JSON only."
                        },
                        {
                            "role": "user",
                            "content": retry_prompt
                        }
                    ],
                    format="json",
                    options={
                        "temperature": 0,
                        "num_ctx": 2048,
                        "num_predict": 1024
                    }
                )

                retry_text = (
                    retry_response["message"]["content"].strip()
                )

                retry_result = json.loads(retry_text)

                if (
                    isinstance(retry_result, dict)
                    and isinstance(
                        retry_result.get("translations"),
                        dict
                    )
                ):
                    retry_result = retry_result["translations"]

                for key in missing:
                    if (
                        key in retry_result
                        and isinstance(retry_result[key], str)
                    ):
                        result[key] = retry_result[key].strip()

            elapsed = time.perf_counter() - start

            print(
                f"[Ollama Batch] "
                f"{len(result)}/{len(json_data)} translated | "
                f"{elapsed:.2f}s"
            )

            return result

        except json.JSONDecodeError as ex:
            print(f"[Batch JSON Error] {ex}")
            print(f"[Raw]: {text[:1000]}")
            return {}

        except Exception as ex:
            print(f"[Batch Error] {ex}")
            return {}