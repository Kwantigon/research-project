# SPARQL query task sheet – Group A (experts)

You are given a [data specification](https://tool.dataspecer.com/api/preview/en/index.html?iri=d51bc125-f2f7-484b-8a57-f8f2b7291d69) about tourist destinations.

Below are three questions written in natural language.
Your task is to write a SPARQL query for each question.

---

## Instructions

1. Work on one question at a time, in order.
2. Before starting each question, start a stopwatch.
3. When you finish writing your query, stop the stopwatch and record the time taken.
4. Record the time taken under each question.
5. Write your query in the space provided.

If you cannot solve a question within ~8 minutes, stop and move on.

If you do not have a stopwatch, you can use an online one.
For example https://www.timeanddate.com/stopwatch/

---

## Question 1

**List all tourist destinations that have a capacity of at least 100 people.**

**Write your SPARQL query here:**

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
SELECT ?place WHERE {
  ?place a <https://slovník.gov.cz/datový/turistické-cíle/pojem/turistický-cíl> ;
    <https://slovník.gov.cz/datový/turistické-cíle/pojem/kapacita> ?q .
  ?q <https://slovník.gov.cz/generický/množství/pojem/hodnota> ?capacity .
  FILTER ( ?capacity > 99 )
}
```

Time taken: 3 minutes 30 seconds

**Notes**

- 2:20 první verze, ale nefiltruje
- 3:30 final, měl jsem špatně název proměnné ve filter.

## Question 2

**List all tourist destinations that have barrier-free access with an elevator at the main entrance.**

*Note: Some attributes, such as whether an elevator exists at the main entrance, are expressed as simple string "true" or "false".*

Write your SPARQL query here:

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
SELECT ?place WHERE {
  ?place a <https://slovník.gov.cz/datový/turistické-cíle/pojem/turistický-cíl> ;
    <https://slovník.gov.cz/datový/turistické-cíle/pojem/bezbariérovost> ?bezbariérovost .
  ?bezbariérovost <https://slovník.gov.cz/generický/bezbariérové-přístupy/pojem/výtah-u-hlavního-vstupu> ?entraceElevator.
  FILTER (?entraceElevator = "true") 
}
```

**Notes**

Vlastně jsem nečetl tu specifikaci, hledal jsem rovnou pojem "elevator", tam jsem vzal ten predikát.
Pak mi trvalo se dostat zpět na ?place, což jsem udělal tak, že jsem si rozbalil ten pattern před tím.

Time taken: 2 minutes 34 seconds

## Question 3

**List all tourist destinations that have an elevator in the interior. For each destination, optionally list whether smoking is allowed or not.**

*Note 1: Some attributes, such as whether an elevator exists in the interior, aare expressed as simple string "true" or "false".*

*Note 2: Even if a destination does not have smoking allowance defined, it should still be in the output.*

Write your SPARQL query here:

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
SELECT ?place ?isSmokingAllowed WHERE {
  ?place a <https://slovník.gov.cz/datový/turistické-cíle/pojem/turistický-cíl> ;
    <https://slovník.gov.cz/datový/turistické-cíle/pojem/bezbariérovost> ?bezbariérovost .
    
  ?bezbariérovost <https://slovník.gov.cz/generický/bezbariérové-přístupy/pojem/výtah-v-interiéru> ?interriorElevator.
  FILTER (?interriorElevator = "true")
  
  OPTIONAL {
    ?place <https://slovník.gov.cz/datový/turistické-cíle/pojem/kouření-povoleno> ?isSmokingAllowed . 
  }
}
```

Time taken: 2 minutes 30 seconds

**Notes**

Celkově mě zaujalo, že vlastně nečtu tu specifikaci - jdu prostě po tom co potřebuju.
