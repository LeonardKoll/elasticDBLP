import threading

import lxml.etree
import json
import requests


def worker(iterator, lock):

    while True:

        with lock:
            event, element = next(iterator)

        # API Call
        pubData = {"type": element.tag}
        for child in element:
            pubData[child.tag] = child.text
        response = requests.post(url, data=json.dumps(pubData), headers=headers)
        print(pubData)
        #print(response.text)

        # Memory Mgmt
        element.clear()
        while element.getprevious() is not None:
            del element.getparent()[0]



# https://stackoverflow.com/questions/12160418/why-is-lxml-etree-iterparse-eating-up-all-my-memory

url = 'http://localhost:9200/dblp/_doc'
headers = {"Content-Type": "application/json"}
pubTypes = (
"data", "article", "inproceedings", "proceedings", "book", "incollection", "phdthesis", "mastersthesis", "www")

count = 0;
iterator = lxml.etree.iterparse("dblp.xml", load_dtd=True, tag=pubTypes);
iteratorLock = threading.Lock()

camilla = threading.Thread(target=worker, args=(iterator, iteratorLock))
camilla.start()
emil = threading.Thread(target=worker, args=(iterator, iteratorLock))
emil.start()
wolfgang = threading.Thread(target=worker, args=(iterator, iteratorLock))
wolfgang.start()
jenny = threading.Thread(target=worker, args=(iterator, iteratorLock))
jenny.start()



"""
for event, element in :

    # API Call
    pubData = {}
    for child in element:
        pubData[child.tag] = child.text
    publication = {element.tag: pubData}
    response = requests.post(url, data=json.dumps(pubData), headers=headers)
    #print(response.text)

    # Some out
    count += 1
    if count % 100 == 0:
        print(count)

    # Memory Mgmt
    element.clear()
    while element.getprevious() is not None:
        del element.getparent()[0]
"""