## Question 1

**List all tourist destinations that have a capacity of at least 100 people.**

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX tc: <https://slovník.gov.cz/datový/turistické-cíle/pojem/>
PREFIX qty: <https://slovník.gov.cz/generický/množství/pojem/>

SELECT DISTINCT *
WHERE {
  ?destination a tc:turistický-cíl ;
               rdfs:label ?label ;
               tc:kapacita ?cap .
  ?cap qty:hodnota ?capacity .
  FILTER(?capacity >= 100)
}
```

Answer: Castle A, Park A.

## Question 2

**List all tourist destinations that have barrier-free access with an elevator at the main entrance.**

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX tc: <https://slovník.gov.cz/datový/turistické-cíle/pojem/>
PREFIX bf: <https://slovník.gov.cz/generický/bezbariérové-přístupy/pojem/>

SELECT DISTINCT *
WHERE {
  ?destination a tc:turistický-cíl ;
               rdfs:label ?label ;
               tc:bezbariérovost ?bf .
  ?bf bf:výtah-u-hlavního-vstupu ?v .
  FILTER(?v = "true") .
}
```

Answer: Castle A, Muzeum A.

## Question 3

**List all tourist destinations that have an elevator in the interior. For each destination, list whether smoking is allowed or not.**

```sparql
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX tc: <https://slovník.gov.cz/datový/turistické-cíle/pojem/>
PREFIX bf: <https://slovník.gov.cz/generický/bezbariérové-přístupy/pojem/>

SELECT *
WHERE {
  ?destination a tc:turistický-cíl ;
               rdfs:label ?label ;
               tc:bezbariérovost ?bf .
  OPTIONAL { ?destination tc:kouření-povoleno ?smokingAllowed } .
  ?bf bf:výtah-v-interiéru ?v .
  FILTER (?v = "true") .
}
```

Answer: Castle A.
