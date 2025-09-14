# SPARQL query task sheet – Group A (experts)

You are given a [data specification](https://tool.dataspecer.com/api/preview/en/index.html?iri=d51bc125-f2f7-484b-8a57-f8f2b7291d69) about tourist destinations.

Below are three questions written in natural language.
Your task is to write a SPARQL query for each of the given question.

## Instructions

1. Work on one question at a time, in order.
2. Before starting each question, start a stopwatch.
3. When you finish writing your query, stop the stopwatch and record the time taken.
4. Write your query in the space provided.
5. Write your recorded time in the space provided.

If you do not have a stopwatch, you can use an [online stopwatch](https://www.timeanddate.com/stopwatch/).

You can test your SPARQL queries using the [local Apache Jena Fuseki web interface](http://localhost:3030/#/dataset/tourist-destination/query).
The instructions for setting it up are provided in the README.md file in the repository.

## Question 1

**List all tourist destinations that have a capacity of at least 100 people.**

This query should retrive:

- http://example.org/destination/castle-A
- http://example.org/destination/park-A

Write your SPARQL query here:

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
SELECT * WHERE {
  ?destination a <https://slovník.gov.cz/datový/turistické-cíle/pojem/turistický-cíl> ;
    <https://slovník.gov.cz/datový/turistické-cíle/pojem/kapacita>/<https://slovník.gov.cz/generický/množství/pojem/hodnota> ?capacity.
  FILTER(?capacity >= 100)
}
```

Time taken: 01 minutes 21 seconds

## Question 2

**List all barrier-free tourist destinations that have an elevator at the main entrance.**

*Note: Some attributes, such as whether an elevator exists at the main entrance, are expressed as a simple string "true" or "false".*

This query should retrive:

- http://example.org/destination/castle-A
- http://example.org/destination/museum-A

Write your SPARQL query here:

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
SELECT * WHERE {
  ?destination a <https://slovník.gov.cz/datový/turistické-cíle/pojem/turistický-cíl> ;
    <https://slovník.gov.cz/datový/turistické-cíle/pojem/bezbariérovost>/<https://slovník.gov.cz/generický/bezbariérové-přístupy/pojem/výtah-u-hlavního-vstupu> ?elevator.
  FILTER(?elevator = "true")
}
```

Time taken: 02 minutes 19 seconds

## Question 3

**List all barrier-free tourist destinations that have an elevator in the interior. For each destination, optionally list whether smoking is allowed or not.**

*Note 1: Some attributes, such as whether smoking is allowed, are expressed as a simple string "true" or "false".*

*Note 2: Even if a destination does not have smoking allowance defined, it should still be in the output.*

This query should retrive:

- http://example.org/destination/castle-A

Write your SPARQL query here:

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
SELECT * WHERE {
  ?destination a <https://slovník.gov.cz/datový/turistické-cíle/pojem/turistický-cíl> ;
    <https://slovník.gov.cz/datový/turistické-cíle/pojem/bezbariérovost>/<https://slovník.gov.cz/generický/bezbariérové-přístupy/pojem/výtah-v-interiéru> ?elevator.
  OPTIONAL {?destination <https://slovník.gov.cz/datový/turistické-cíle/pojem/kouření-povoleno> ?smoking .}
  FILTER(?elevator = "true")
}
```

Time taken: 01 minutes 19 seconds
