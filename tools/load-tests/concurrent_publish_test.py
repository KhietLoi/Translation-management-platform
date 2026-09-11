import asyncio
import aiohttp
import time
import string


API_URL = "http://localhost:5182/api/TranslationPipeline/publish"

TOKEN = ""

HEADERS = {
    "accept": "*/*",
    "Authorization": f"Bearer {TOKEN}",
    "Content-Type": "application/json"
}

PAYLOAD = {
    "projectId": "01a02209-ab7e-7ffe-9185-7ec6956cb56e",
    "notes": "test_v4"
}

# Số request muốn gửi đồng thời
REQUEST_COUNT = 3


def request_name(index: int) -> str:
    """
    0  -> REQUEST-A
    1  -> REQUEST-B
    ...
    25 -> REQUEST-Z
    26 -> REQUEST-AA
    """
    name = ""

    index += 1

    while index > 0:
        index, remainder = divmod(index - 1, 26)
        name = chr(65 + remainder) + name

    return f"REQUEST-{name}"


async def publish(
    session: aiohttp.ClientSession,
    name: str
):
    start_time = time.perf_counter()

    print(f"[{name}] START")

    try:
        async with session.post(
            API_URL,
            json=PAYLOAD,
            headers=HEADERS
        ) as response:

            response_body = await response.text()

            elapsed = (
                time.perf_counter()
                - start_time
            )

            print(
                f"[{name}] "
                f"STATUS={response.status} "
                f"TIME={elapsed:.2f}s"
            )

            print(
                f"[{name}] "
                f"RESPONSE={response_body}"
            )

    except Exception as ex:

        elapsed = (
            time.perf_counter()
            - start_time
        )

        print(
            f"[{name}] "
            f"ERROR after {elapsed:.2f}s: {ex}"
        )


async def main():

    connector = aiohttp.TCPConnector(
        ssl=False
    )

    async with aiohttp.ClientSession(
        connector=connector
    ) as session:

        tasks = [
            publish(
                session,
                request_name(i)
            )
            for i in range(REQUEST_COUNT)
        ]

        print(
            f"Sending {REQUEST_COUNT} "
            f"concurrent requests..."
        )

        start_time = time.perf_counter()

        await asyncio.gather(*tasks)

        elapsed = (
            time.perf_counter()
            - start_time
        )

        print(
            f"\nFinished {REQUEST_COUNT} "
            f"requests in {elapsed:.2f}s"
        )


if __name__ == "__main__":
    asyncio.run(main())