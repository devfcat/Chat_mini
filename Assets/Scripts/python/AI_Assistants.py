import time
import re

client = OpenAI(
    api_key=''
)
assistant = client.beta.assistants.retrieve(
    assistant_id=''
)
thread = client.beta.threads.create()

def wait_on_run(run, thread):
    while run.status == "queued" or run.status == "in_progress":
        run = client.beta.threads.runs.retrieve(
            thread_id=thread.id,
            run_id=run.id,
        )
        time.sleep(0.5)
    return run


def add(a, b):
    return a+b

'''
while True:
    content = input('Enter your message:')
    message = client.beta.threads.messages.create(
        thread_id=thread.id,
        role='user',
        content=content
    )



    # Execute our run
    run = client.beta.threads.runs.create(
        thread_id=thread.id,
        assistant_id=assistant.id,
    )

    # Wait for completion
    wait_on_run(run, thread)
    # Retrieve all the messages added after our last user message
    messages = client.beta.threads.messages.list(
        thread_id=thread.id, order="asc", after=message.id
    )
    response_text = ""
    for message in messages:
        for c in message.content:
            response_text += c.text.value
    clean_text = re.sub('【.*?】', '', response_text)
    print(clean_text)
'''