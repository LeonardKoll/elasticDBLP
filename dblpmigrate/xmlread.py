import lxml.etree
import json
import requests

# https://stackoverflow.com/questions/12160418/why-is-lxml-etree-iterparse-eating-up-all-my-memory

url = 'http://localhost:9200/dblp/_doc'
headers = {"Content-Type": "application/json"}
pubTypes = ("article", "inproceedings", "proceedings", "book", "incollection", "phdthesis", "mastersthesis", "www", "data")

count = 0;
for event, element in lxml.etree.iterparse("dblp.xml", load_dtd=True, tag=pubTypes):

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
