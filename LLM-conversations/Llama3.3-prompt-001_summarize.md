### Prompt

Given the following data specification in RDF, summarize what it is about.
```
@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.
@prefix dct: <http://purl.org/dc/terms/>.
@prefix dsv: <https://w3id.org/dsv#>.
@prefix owl: <http://www.w3.org/2002/07/owl#>.
@prefix skos: <http://www.w3.org/2004/02/skos/core#>.
@prefix vann: <http://purl.org/vocab/vann/>.
@prefix cardinality: <https://w3id.org/dsv/cardinality#>.
@prefix requirement: <https://w3id.org/dsv/requirement-level#>.
@prefix role: <https://w3id.org/dsv/class-role#>.


<https://techlib.cz/profile/ccmm/applicationProfileConceptualModel> a dsv:ConceptualModel.

<https://techlib.cz/profile/ccmm/Dataset> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Conjunto de datos"@es, "Dataset"@en, "Dataset"@it, "Datová sada"@cs, "Jeu de données"@fr, "Σύνολο Δεδομένων"@el, "قائمة بيانات"@ar, "データセット"@ja, "Datasæt"@da;
    skos:definition "1つのエージェントによって公開またはキュレートされ、1つ以上の形式でアクセスまたはダウンロードできるデータの集合。"@ja, "A collection of data, published or curated by a single source, and available for access or download in one or more representations."@en, "Kolekce dat poskytovaná či řízená jedním zdrojem, která je k dispozici pro přístup či stažení v jednom či více formátech."@cs, "Raccolta di dati, pubblicati o curati da un'unica fonte, disponibili per l'accesso o il download in uno o più formati."@it, "Una colección de datos, publicados o conservados por una única fuente, y disponibles para ser accedidos o descargados en uno o más formatos."@es, "Une collection de données, publiée ou élaborée par une seule source, et disponible pour accès ou téléchargement dans un ou plusieurs formats."@fr, "Μία συλλογή από δεδομένα, δημοσιευμένη ή επιμελημένη από μία και μόνο πηγή, διαθέσιμη δε προς πρόσβαση ή μεταφόρτωση σε μία ή περισσότερες μορφές."@el, "قائمة بيانات منشورة أو مجموعة من قبل مصدر ما و متاح الوصول إليها أو تحميلها"@ar, "En samling af data, udgivet eller udvalgt og arrangeret af en enkelt kilde og som er til råde for adgang til eller download af i en eller flere repræsentationer."@da;
    a dsv:ClassProfile;
    dsv:class <http://www.w3.org/ns/dcat#Dataset>.

<https://techlib.cz/profile/ccmm/Dataset.title> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Title"@en;
    skos:definition "A name given to the resource."@en;
    dsv:cardinality cardinality:11;
    dsv:property dct:title;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Dataset.resourceType> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Resource type"@en;
    skos:definition "A description of the resource. "@en;
    vann:usageNote "Gets the value from DataCite controlled list https://datacite-metadata-schema.readthedocs.io/en/4.6/appendices/appendix-1/resourceTypeGeneral/."@en;
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/datacite/resourceType>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Dataset.hasAlternateIdentifier> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/hasAlternateIdentifier>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/hasAlternateIdentifier>
];
    dsv:property <https://techlib.cz/vocabulary/datacite/hasAlternateIdentifier>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <plckn6gl38m7ufbi8u>.

<https://techlib.cz/profile/ccmm/Dataset.primaryLanguage> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Primary language"@en;
    skos:definition "Primary language of a dataset."@en;
    vann:usageNote "Use for the language in which is the dataset created and published."@en;
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/datacite/language>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Dataset.otherLanguage> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Other language"@en;
    vann:usageNote "Other languages in which is the dataset available."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/language>
];
    dsv:cardinality cardinality:0n;
    dsv:property <https://techlib.cz/vocabulary/datacite/language>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Dataset.description> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Description"@en;
    vann:usageNote "Description of a data set other than abstract, e.g. purpose of creation, sources, preffered usage etc."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <:undefined-identifier:>
];
    dsv:cardinality cardinality:0n;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <https://ofn.gov.cz/zdroj/základní-datové-typy/2020-07-01/text>.

<https://techlib.cz/profile/ccmm/Dataset.version> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Version"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <:undefined-identifier:>
];
    dsv:cardinality cardinality:0n;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Dataset.publicationYear> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Publication year"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/relatedItemPublicationYear>
];
    dsv:cardinality cardinality:11;
    dsv:property <https://techlib.cz/vocabulary/datacite/relatedItemPublicationYear>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#gYear>.

<https://techlib.cz/profile/ccmm/Dataset.hasRelatedResource> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has related resource"@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasRelatedResource>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Resource>.

<https://techlib.cz/profile/ccmm/Dataset.hasTermsOfUse> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has terms of use"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/hasVersion>
];
    dsv:cardinality cardinality:11;
    dsv:property <https://techlib.cz/vocabulary/datacite/hasVersion>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/TermsOfUse>.

<https://techlib.cz/profile/ccmm/Dataset.hasFundingReference> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/hasFundingReference>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/hasFundingReference>
];
    dsv:cardinality cardinality:0n;
    dsv:property <https://techlib.cz/vocabulary/datacite/hasFundingReference>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/FundingReference>.

<https://techlib.cz/profile/ccmm/Dataset.hasDistribution> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has distribution"@en;
    vann:usageNote "For the distribution of data in the form of files use instances of class Distribution – downloadable file. For the distribution of data as a service use instance of class Distribution – data service. "@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasDistribution>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Distribution>.

<https://techlib.cz/profile/ccmm/Dataset.hasValidationResult> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has validation result"@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasValidationResult>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ValidationResult>.

<https://techlib.cz/profile/ccmm/Dataset.hasSubject> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    vann:usageNote ""@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/hasSubject>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/hasSubject>
];
    dsv:property <https://techlib.cz/vocabulary/datacite/hasSubject>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Subject>.

<https://techlib.cz/profile/ccmm/Dataset.hasTimeReference> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has time reference"@en;
    vann:usageNote "Must contain at least one time reference with date type value \"Created\" from DataCIte controlled vocabulary https://datacite-metadata-schema.readthedocs.io/en/4.5/appendices/appendix-1/dateType/."@en;
    dsv:cardinality cardinality:1n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasTimeReference>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/TimeReference>.

<https://techlib.cz/profile/ccmm/Dataset.qualifiedRelation> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "qualified relation"@en;
    skos:definition "Attachment of resource to the relationship."@en;
    vann:usageNote "Description of relationship between dataset and agent (e.g. creator, publisher, etc.). Controlled list of possible relations will be created soon. In the meantime use creator, publisher, or one from list of contributorType  from DataCite: https://datacite-metadata-schema.readthedocs.io/en/4.6/appendices/appendix-1/contributorType/)"@en;
    dsv:cardinality dsv:nn;
    dsv:property <https://techlib.cz/vocabulary/ccmm/qualifiedRelation>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ResourceToAgentRelationship>.

<https://techlib.cz/profile/ccmm/Dataset.provenance> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Dataset.provenance>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Dataset.provenance>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Dataset.provenance>
];
    dsv:cardinality cardinality:0n;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ProvenanceStatement>.

<https://techlib.cz/profile/ccmm/Dataset.hasLocation> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has location"@en;
    vann:usageNote "In the CCMM spatial coverage does not only cover the area of dataset, it may be connected to the dataset in some other way, e.g. the place from where the data were measured, administrative area to which the dataset  conforms or other."@en;
    dsv:profileOf <https://mff-uk.github.io/specifications/dcat-dap/#Dataset.spatial-geographicalCoverage>;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Dataset.geographicalCoverage>
];
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Location>.

<https://techlib.cz/profile/ccmm/Dataset.hasIdentifier> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has identifier"@en;
    skos:definition "has identifier"@en;
    dsv:cardinality cardinality:1n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasIdentifier>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Identifier>.

<https://techlib.cz/profile/ccmm/Dataset.isDescribedBy> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "is described by"@en;
    skos:definition "Inverse relation between dataset and metadata record."@en;
    dsv:cardinality cardinality:1n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/isDescribedBy>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/MetadataRecord>.

<https://techlib.cz/profile/ccmm/Dataset.hasAlternateTitle> dsv:domain <https://techlib.cz/profile/ccmm/Dataset>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has alternate title"@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasAlternateTitle>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/AlternateTitle>.

<https://techlib.cz/profile/ccmm/Agent> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Agent>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Agent>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Agent>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/Agent.contactPoint> dsv:domain <https://techlib.cz/profile/ccmm/Agent>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <http://www.w3.org/ns/dcat#contactPoint>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <http://www.w3.org/ns/dcat#contactPoint>
];
    dsv:cardinality cardinality:0n;
    dsv:property <http://www.w3.org/ns/dcat#contactPoint>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ContactDetails>.

<https://techlib.cz/profile/ccmm/Agent.hasIdentifier> dsv:domain <https://techlib.cz/profile/ccmm/Agent>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has identifier"@en;
    skos:definition "has identifier"@en;
    dsv:cardinality cardinality:0n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasIdentifier>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Identifier>.

<https://techlib.cz/profile/ccmm/Person> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "Physical person. "@en;
    vann:usageNote ""@en;
    dsv:specializes <https://techlib.cz/profile/ccmm/Agent>;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <http://www.w3.org/ns/prov#Person>
];
    a dsv:ClassProfile;
    dsv:class <http://www.w3.org/ns/prov#Person>.

<https://techlib.cz/profile/ccmm/Person.GivenName> dsv:domain <https://techlib.cz/profile/ccmm/Person>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Given name"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <http://www.w3.org/2006/vcard/ns#given-name>
];
    dsv:cardinality cardinality:0n;
    dsv:property <http://www.w3.org/2006/vcard/ns#given-name>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Person.FamilyName> dsv:domain <https://techlib.cz/profile/ccmm/Person>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Family name"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <http://www.w3.org/2006/vcard/ns#family-name>
];
    dsv:cardinality cardinality:0n;
    dsv:property <http://www.w3.org/2006/vcard/ns#family-name>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Person.hasAffiliation> dsv:domain <https://techlib.cz/profile/ccmm/Person>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has affiliation"@en;
    skos:definition "Affiliation of the person to the organization."@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasAffiliation>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Organization>.

<https://techlib.cz/profile/ccmm/Person.fullName> dsv:domain <https://techlib.cz/profile/ccmm/Person>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Full name"@en;
    skos:definition "To specify the organizational name associated with the object"@en;
    dsv:cardinality cardinality:11;
    dsv:property <http://xmlns.com/foaf/0.1/name>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Organization> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "Represents a collection of people organized together into a community or other social, commercial or political structure. The group has some common purpose or reason for existence which goes beyond the set of people belonging to it and can act as an Agent. Organizations are often decomposable into hierarchical structures. "@en;
    dsv:specializes <https://techlib.cz/profile/ccmm/Agent>;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <http://www.w3.org/ns/prov#Organization>
];
    a dsv:ClassProfile;
    dsv:class <http://www.w3.org/ns/prov#Organization>.

<https://techlib.cz/profile/ccmm/Organization.Name> dsv:domain <https://techlib.cz/profile/ccmm/Organization>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Name"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <http://www.w3.org/2006/vcard/ns#organization-name>
];
    dsv:cardinality cardinality:11;
    dsv:property <http://www.w3.org/2006/vcard/ns#organization-name>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Organization.AlternateName> dsv:domain <https://techlib.cz/profile/ccmm/Organization>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Alternate name"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <http://www.w3.org/2006/vcard/ns#organization-name>
];
    dsv:cardinality cardinality:0n;
    dsv:property <http://www.w3.org/2006/vcard/ns#organization-name>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <https://ofn.gov.cz/zdroj/základní-datové-typy/2020-07-01/text>.

<https://techlib.cz/profile/ccmm/TermsOfUse> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Terms of use"@en;
    skos:definition "Terms of use defines overall conditions and rights regarding access and usage  of dataset. "@en;
    vann:usageNote "Class terms of use combines to classes from profiles DCAT-AP-CZ and DataCite. It shall be further extended. "@en;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/datacite/Rights>.

<https://techlib.cz/profile/ccmm/TermsOfUse.accessRights> dsv:domain <https://techlib.cz/profile/ccmm/TermsOfUse>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Access rights"@en;
    skos:definition "Information about who access the resource or an indication of its security status."@en;
    dsv:cardinality cardinality:11;
    dsv:property dct:accessRights;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/TermsOfUse.License> dsv:domain <https://techlib.cz/profile/ccmm/TermsOfUse>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "Information about licensing details of the resource."@en;
    vann:usageNote ""@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource dct:license
];
    dsv:cardinality cardinality:0n;
    dsv:property dct:license;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/TermsOfUse.Description> dsv:domain <https://techlib.cz/profile/ccmm/TermsOfUse>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "Descriptive text on details of rights and licensing."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource dct:description
];
    dsv:cardinality cardinality:0n;
    dsv:property dct:description;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <https://ofn.gov.cz/zdroj/základní-datové-typy/2020-07-01/text>.

<https://techlib.cz/profile/ccmm/TermsOfUse.contactPoint> dsv:domain <https://techlib.cz/profile/ccmm/TermsOfUse>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Punto de contacto"@es, "has contact"@en, "kontaktní bod"@cs, "point de contact"@fr, "punto di contatto"@it, "σημείο επικοινωνίας"@el, "عنوان اتصال"@ar, "窓口"@ja, "kontaktpunkt"@da;
    skos:definition "Información relevante de contacto para el recurso catalogado. Se recomienda el uso de vCard."@es, "Informazioni di contatto rilevanti per la risorsa catalogata. Si raccomanda l'uso di vCard."@it, "Contact person for the further details about the terms of use."@en, "Relevantní kontaktní informace pro katalogizovaný zdroj. Doporučuje se použít slovník VCard."@cs, "Relie un jeu de données à une information de contact utile en utilisant VCard."@fr, "Συνδέει ένα σύνολο δεδομένων με ένα σχετικό σημείο επικοινωνίας, μέσω VCard."@el, "تربط قائمة البيانات بعنوان اتصال موصف  باستخدام VCard"@ar, "データセットを、VCardを用いて提供されている適切な連絡先情報にリンクします。"@ja, "Relevante kontaktoplysninger for den katalogiserede ressource. Anvendelse af vCard anbefales."@da;
    dsv:property <http://www.w3.org/ns/dcat#contactPoint>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Agent>.

<https://techlib.cz/profile/ccmm/FundingReference> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/FundingReference>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/FundingReference>
];
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/datacite/FundingReference>.

<https://techlib.cz/profile/ccmm/FundingReference.localIdentifier> dsv:domain <https://techlib.cz/profile/ccmm/FundingReference>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Local identifier"@en;
    skos:definition "The code assigned by the funder to a sponsored award (grant)."@en;
    vann:usageNote "In case the award or grant has an ID or DOI, the full URL can be included."@en;
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/datacite/awardNumber>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/FundingReference.awardTitle> dsv:domain <https://techlib.cz/profile/ccmm/FundingReference>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Award title"@en;
    skos:definition " The human readable title or name of the award (grant)."@en;
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/datacite/awardTitle>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/FundingReference.hasFunder> dsv:domain <https://techlib.cz/profile/ccmm/FundingReference>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has funder"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/hasFunderIdentifier>
];
    dsv:cardinality cardinality:1n;
    dsv:property <https://techlib.cz/vocabulary/datacite/hasFunderIdentifier>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/FunderIdentifier>.

<https://techlib.cz/profile/ccmm/FundingReference.fundingProgram> dsv:domain <https://techlib.cz/profile/ccmm/FundingReference>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Funding program"@en;
    skos:definition "Reference to the specific funding program."@en;
    dsv:cardinality cardinality:01;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#anyURI>.

<https://techlib.cz/profile/ccmm/Distribution> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/Distribution.title> dsv:domain <https://techlib.cz/profile/ccmm/Distribution>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Title"@en;
    dsv:profileOf <https://mff-uk.github.io/specifications/dcat-dap/#Distribution.title>;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-dap/#Distribution.title>
];
    dsv:cardinality cardinality:11;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/ValidationResult> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Validation result"@en;
    skos:definition "Class describing the result of validation testing. It may include validation of metadata, data and/or web services accessing the data."@en;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/ccmm/ValidationResult>.

<https://techlib.cz/profile/ccmm/ValidationResult.label> dsv:domain <https://techlib.cz/profile/ccmm/ValidationResult>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Validation result label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Subject> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    vann:usageNote "Recommended approach is to use controlled list values, preferably in the form of IRI. "@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/Subject>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/Subject>
];
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/datacite/Subject>.

<https://techlib.cz/profile/ccmm/Subject.inSubjectScheme> dsv:domain <https://techlib.cz/profile/ccmm/Subject>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "in subject scheme"@en;
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/ccmm/inSubjectScheme>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/SubjectScheme>.

<https://techlib.cz/profile/ccmm/Subject.classificationCode> dsv:domain <https://techlib.cz/profile/ccmm/Subject>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Classification code"@en;
    skos:definition "The classification code used for the subject term in the subject scheme."@en;
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/datacite/subjectClassificationCode>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Subject.title> dsv:domain <https://techlib.cz/profile/ccmm/Subject>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Title"@en;
    skos:definition "Title of the subject"@en;
    dsv:cardinality cardinality:11;
    dsv:property dct:title;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Subject.definition> dsv:domain <https://techlib.cz/profile/ccmm/Subject>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Definition"@en;
    skos:definition "Definition of the subject."@en;
    dsv:cardinality cardinality:01;
    dsv:property skos:definition;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <https://ofn.gov.cz/zdroj/základní-datové-typy/2020-07-01/text>.

<https://techlib.cz/profile/ccmm/Resource> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Resource"@en;
    skos:definition "Resource represents any resource, physical or digital, that is related to the described dataset. "@en;
    vann:usageNote "Class is used for description of the resource together with the type of relation, for which is used codelist. Distributions of the datasets SHALL NOT be modelled as related resource, but as a distribution. Use qualified resource property to link to the dataset class. "@en;
    a dsv:ClassProfile;
    dsv:class rdfs:Resource, <https://techlib.cz/vocabulary/datacite/Resource>.

<https://techlib.cz/profile/ccmm/Resource.title> dsv:domain <https://techlib.cz/profile/ccmm/Resource>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Title"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/relatedItemTitle>
];
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/datacite/relatedItemTitle>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Resource.type> dsv:domain <https://techlib.cz/profile/ccmm/Resource>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    vann:usageNote "Contains value from controlled list."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource dct:type
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/resourceType>
];
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/datacite/resourceType>, dct:type;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Resource.relationType> dsv:domain <https://techlib.cz/profile/ccmm/Resource>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Relation type"@en;
    skos:definition "Type of the relation between Dataset and related resource."@en;
    vann:usageNote "Use value from controlled list."@en;
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/ccmm/relationType>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Resource.hasTimeReference> dsv:domain <https://techlib.cz/profile/ccmm/Resource>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has time reference"@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasTimeReference>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/TimeReference>.

<https://techlib.cz/profile/ccmm/Resource.qualifiedRelation> dsv:domain <https://techlib.cz/profile/ccmm/Resource>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "qualified relation"@en;
    skos:definition "Attachment of resource to the relationship."@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/qualifiedRelation>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ResourceToAgentRelationship>.

<https://techlib.cz/profile/ccmm/Resource.hasIdentifier> dsv:domain <https://techlib.cz/profile/ccmm/Resource>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has identifier"@en;
    skos:definition "has identifier"@en;
    dsv:cardinality cardinality:0n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasIdentifier>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Identifier>.

<https://techlib.cz/profile/ccmm/Resource.resourceURL> dsv:domain <https://techlib.cz/profile/ccmm/Resource>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Resource URL"@en;
    skos:definition "Resolvable URL representing the resource, preferably human readable."@en;
    vann:usageNote "In case it is filled, it must be filled the same in c:iri.\nIf more resolvable IRIs, Resource URL shall link to human readable version, machine readable ID is always in c:iri."@en;
    dsv:cardinality cardinality:01;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdfs:Literal.

<https://techlib.cz/profile/ccmm/TimeReference> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Time reference"@en;
    skos:definition "A date on which something relative to the resource has happened."@en;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/ccmm/TimeReference>.

<https://techlib.cz/profile/ccmm/TimeReference.date> dsv:domain <https://techlib.cz/profile/ccmm/TimeReference>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Date"@en;
    skos:definition "A point or period of time associated with an event in the lifecycle of the resource."@en;
    dsv:cardinality cardinality:11;
    dsv:property <https://techlib.cz/vocabulary/datacite/dateValue>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#date>.

<https://techlib.cz/profile/ccmm/TimeReference.dateType> dsv:domain <https://techlib.cz/profile/ccmm/TimeReference>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Date type"@en;
    skos:definition "Type of the date."@en;
    vann:usageNote "Use value from controlled list https://datacite-metadata-schema.readthedocs.io/en/4.6/appendices/appendix-1/dateType/."@en;
    dsv:cardinality cardinality:11;
    dsv:property <https://techlib.cz/vocabulary/datacite/dateType>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/TimeReference.dateInformation> dsv:domain <https://techlib.cz/profile/ccmm/TimeReference>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Date information"@en;
    skos:definition "Additional information to the date in text form."@en;
    dsv:cardinality cardinality:01;
    dsv:property <https://techlib.cz/vocabulary/datacite/dateInformation>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <https://ofn.gov.cz/zdroj/základní-datové-typy/2020-07-01/text>.

<https://techlib.cz/profile/ccmm/ResourceToAgentRelationship> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Resource to agent relationship"@en;
    skos:definition "An association class for attaching additional information to a relationship between Dataset and Agent."@en;
    vann:usageNote "Use to characterize a relationship between datasets and agents, where the nature of the relationship is known but is not adequately characterized by the standard [DCTERMS] properties (dcterms:hasPart, dcterms:isPartOf, dcterms:conformsTo, dcterms:isFormatOf, dcterms:hasFormat, dcterms:isVersionOf, dcterms:hasVersion, dcterms:replaces, dcterms:isReplacedBy, dcterms:references, dcterms:isReferencedBy, dcterms:requires, dcterms:isRequiredBy) or [PROV-O] properties (prov:wasDerivedFrom, prov:wasInfluencedBy, prov:wasQuotedFrom, prov:wasRevisionOf, prov:hadPrimarySource, prov:alternateOf, prov:specializationOf)"@en;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/ccmm/ResourceToAgentRelationship>.

<https://techlib.cz/profile/ccmm/ResourceToAgentRelationship.relation> dsv:domain <https://techlib.cz/profile/ccmm/ResourceToAgentRelationship>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "relation"@en;
    dsv:cardinality cardinality:11;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Agent>.

<https://techlib.cz/profile/ccmm/DatasetToAgentRelationship.hadRole> dsv:domain <https://techlib.cz/profile/ccmm/ResourceToAgentRelationship>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Relationship.hadRole>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Relationship.hadRole>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Relationship.hadRole>
];
    dsv:cardinality cardinality:11;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ResourceAgentRoleType>.

<https://techlib.cz/profile/ccmm/ResourceAgentRoleType> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Resource agent role type"@en;
    vann:usageNote "List of roles of agents in relation to resources or specifically datasets."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Concept>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/ResourceAgentRoleType.label> dsv:domain <https://techlib.cz/profile/ccmm/ResourceAgentRoleType>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "label"@en;
    skos:definition "Resource agent role type label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdfs:Literal.

<https://techlib.cz/profile/ccmm/FunderIdentifier> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/FunderIdentifier>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://techlib.cz/vocabulary/datacite/FunderIdentifier>
];
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/datacite/FunderIdentifier>.

<https://techlib.cz/profile/ccmm/FunderIdentifier.funderName> dsv:domain <https://techlib.cz/profile/ccmm/FunderIdentifier>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Funder name"@en;
    skos:definition "Funder name"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/FunderIdentifier.hasIdentifier> dsv:domain <https://techlib.cz/profile/ccmm/FunderIdentifier>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has identifier"@en;
    skos:definition "has identifier"@en;
    dsv:cardinality cardinality:0n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasIdentifier>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Identifier>.

<https://techlib.cz/profile/ccmm/ContactDetails> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Contact details"@en;
    skos:definition "Contact details such as telephone, e-mail address etc."@en;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/ccmm/ContactDetails>.

<https://techlib.cz/profile/ccmm/ContactDetails.phone> dsv:domain <https://techlib.cz/profile/ccmm/ContactDetails>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Phone"@en;
    skos:definition "Phone number"@en;
    vann:usageNote "Phone number in international format +xx(x) xxx xxx xxx"@en;
    dsv:cardinality cardinality:0n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/phone>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/ContactDetails.email> dsv:domain <https://techlib.cz/profile/ccmm/ContactDetails>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Email"@en;
    skos:definition "Email address."@en;
    vann:usageNote "Email address in standard format, e.g. john.doe@domain.com"@en;
    dsv:cardinality cardinality:0n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/email>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/ContactDetails.hasAddress> dsv:domain <https://techlib.cz/profile/ccmm/ContactDetails>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <http://www.w3.org/2006/vcard/ns#hasAddress>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <http://www.w3.org/2006/vcard/ns#hasAddress>
];
    dsv:cardinality cardinality:0n;
    dsv:property <http://www.w3.org/2006/vcard/ns#hasAddress>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Address>.

<https://techlib.cz/profile/ccmm/ContactDetails.dataBox> dsv:domain <https://techlib.cz/profile/ccmm/ContactDetails>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Data box"@en;
    skos:definition "Code of the data box."@en;
    vann:usageNote ""@en;
    dsv:cardinality cardinality:0n;
    dsv:property <https://techlib.cz/vocabulary/ccmm/dataBox>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Address> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <http://www.w3.org/ns/locn#Address>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <http://www.w3.org/ns/locn#Address>
];
    a dsv:ClassProfile;
    dsv:class <http://www.w3.org/ns/locn#Address>.

<https://techlib.cz/profile/ccmm/Address.label> dsv:domain <https://techlib.cz/profile/ccmm/Address>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Address label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/MetadataRecord> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Metadata Record"@en, "Katalogizační záznam"@cs, "Record di catalogo"@it, "Registre du catalogue"@fr, "Registro del catálogo"@es, "Καταγραφή καταλόγου"@el, "سجل"@ar, "カタログ・レコード"@ja, "Katalogpost"@da;
    skos:definition "1つのデータセットを記述したデータ・カタログ内のレコード。"@ja, "Description of the metadata record of the dataset. "@en, "Un record in un catalogo di dati che descrive un singolo dataset o servizio di dati."@it, "Un registre du catalogue ou une entrée du catalogue, décrivant un seul jeu de données."@fr, "Un registro en un catálogo de datos que describe un solo conjunto de datos o un servicio de datos."@es, "Záznam v datovém katalogu popisující jednu datovou sadu či datovou službu."@cs, "Μία καταγραφή ενός καταλόγου, η οποία περιγράφει ένα συγκεκριμένο σύνολο δεδομένων."@el, "En post i et datakatalog der beskriver registreringen af et enkelt datasæt eller en datatjeneste."@da;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#CatalogueRecord>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/MetadataRecord.conformsToStandard> dsv:domain <https://techlib.cz/profile/ccmm/MetadataRecord>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Conforms to standard"@en;
    skos:definition "An established standard to which the metadata record conforms."@en;
    dsv:profileOf <https://mff-uk.github.io/specifications/dcat-dap/#CataloguedResource.conformsTo>;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-dap/#CataloguedResource.conformsTo>
];
    dsv:cardinality cardinality:0n;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ApplicationProfile>.

<https://techlib.cz/profile/ccmm/MetadataRecord.Language> dsv:domain <https://techlib.cz/profile/ccmm/MetadataRecord>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "Language of the metadata record"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource dct:language
];
    dsv:cardinality cardinality:0n;
    dsv:property dct:language;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/MetadataRecord.originalRepository> dsv:domain <https://techlib.cz/profile/ccmm/MetadataRecord>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "original repository"@en;
    skos:definition "Link to the repository from which the metadata were originally stored and curated."@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/originalRepository>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Repository>.

<https://techlib.cz/profile/ccmm/MetadataRecord.DateCreated> dsv:domain <https://techlib.cz/profile/ccmm/MetadataRecord>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Date created"@en;
    skos:definition "The date on which the metadata record was created."@en;
    dsv:cardinality cardinality:01;
    dsv:property dct:created;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#date>.

<https://techlib.cz/profile/ccmm/MetadataRecord.DateUpdated> dsv:domain <https://techlib.cz/profile/ccmm/MetadataRecord>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Date updated"@en;
    vann:usageNote "The most recent date on which the metadata record was changed or modified."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource dct:modified
];
    dsv:cardinality cardinality:0n;
    dsv:property dct:modified;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#date>.

<https://techlib.cz/profile/ccmm/MetadataRecord.describes> dsv:domain <https://techlib.cz/profile/ccmm/MetadataRecord>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Describes"@en;
    skos:definition "Dataset described by the metadata record."@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/describes>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Dataset>.

<https://techlib.cz/profile/ccmm/MetadataRecord.qualifiedRelation> dsv:domain <https://techlib.cz/profile/ccmm/MetadataRecord>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "Enlace a una descripción de la relación con otro recurso."@es, "Link a una descrizione di una relazione con un'altra risorsa."@it, "Link to a description of a relationship between metadata record and agent."@en, "Odkaz na popis vztahu s jiným zdrojem."@cs, "Reference til en beskrivelse af en relation til en anden ressource."@da;
    vann:usageNote "Relationship shall contain at least one agent with role \"DataManager\" from codelist https://datacite-metadata-schema.readthedocs.io/en/4.5/appendices/appendix-1/contributorType/."@en;
    dsv:profileOf <https://mff-uk.github.io/specifications/dcat-dap/#CataloguedResource.qualifiedRelation>;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-dap/#CataloguedResource.qualifiedRelation>
];
    dsv:cardinality cardinality:1n;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ResourceToAgentRelationship>.

<https://techlib.cz/profile/ccmm/ApplicationProfile> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Application profile"@en;
    skos:definition "A standard or other specification to which a metadata record conforms."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Standard>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/ApplicationProfile.label> dsv:domain <https://techlib.cz/profile/ccmm/ApplicationProfile>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Label of application profile"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/ProvenanceStatement> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#ProvenanceStatement>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#ProvenanceStatement>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#ProvenanceStatement>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/ProvenanceStatement.label> dsv:domain <https://techlib.cz/profile/ccmm/ProvenanceStatement>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Provenance state label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Repository> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Repository"@en, "Catalogo"@it, "Catalogue"@fr, "Catálogo"@es, "Katalog"@cs, "Κατάλογος"@el, "فهرس قوائم البيانات"@ar, "カタログ"@ja, "Katalog"@da;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Catalogue>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Catalogue>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/Repository.label> dsv:domain <https://techlib.cz/profile/ccmm/Repository>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Repository label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Distribution-DownloadableFile> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Distribution - downloadable file"@en;
    skos:definition "Physical embodiment of the dataset in a particular format. "@en;
    dsv:specializes <https://techlib.cz/profile/ccmm/Distribution>;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/ccmm/Distribution-DownloadableFile>.

<https://techlib.cz/profile/ccmm/Distribution-DownloadableFile.downloadUrl> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DownloadableFile>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.downloadUrl>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.downloadUrl>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.downloadUrl>
];
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/File>.

<https://techlib.cz/profile/ccmm/Distribution-DownloadableFile.accessUrl> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DownloadableFile>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.accessUrl>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.accessUrl>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.accessUrl>
];
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/File>.

<https://techlib.cz/profile/ccmm/Distribution-DownloadableFile.Format> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DownloadableFile>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "The file format of the Distribution. "@en;
    vann:usageNote "Identifier from IANA list."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource dct:format
];
    dsv:cardinality cardinality:11;
    dsv:property dct:format;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#anyURI>.

<https://techlib.cz/profile/ccmm/Distribution-DownloadableFile.mediaType> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DownloadableFile>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.mediaType>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.mediaType>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.mediaType>
];
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/MediaType>.

<https://techlib.cz/profile/ccmm/Distribution-DownloadableFile.byteSize> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DownloadableFile>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Byte size"@en;
    skos:definition "The size of a Distribution in bytes. "@en;
    dsv:cardinality cardinality:11;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#integer>.

<https://techlib.cz/profile/ccmm/Distribution-DownloadableFile.conformsToSchema> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DownloadableFile>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "conforms to schema"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.linkedSchemas>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.linkedSchemas>
];
    dsv:cardinality cardinality:0n;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ApplicationProfile>.

<https://techlib.cz/profile/ccmm/Distribution-DataService.checksum> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DownloadableFile>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.checksum>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.checksum>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.checksum>
];
    dsv:cardinality cardinality:01;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Checksum>.

<https://techlib.cz/profile/ccmm/Distribution-DataService> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Distribution - data service"@en;
    skos:definition "Physical embodiment of the dataset as a particular data service. "@en;
    dsv:specializes <https://techlib.cz/profile/ccmm/Distribution>;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/ccmm/Distribution-DataService>.

<https://techlib.cz/profile/ccmm/Distribution-DataService.accessService> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DataService>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.accessService>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.accessService>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.accessService>
];
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/DataService>.

<https://techlib.cz/profile/ccmm/Distribution-DataService.Description> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DataService>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Description"@en;
    skos:definition "A free-text account of the Distribution."@en;
    vann:usageNote "This property can be repeated for parallel language versions of the description. "@en;
    dsv:cardinality cardinality:0n;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <https://ofn.gov.cz/zdroj/základní-datové-typy/2020-07-01/text>.

<https://techlib.cz/profile/ccmm/Distribution-DataService.specification> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DataService>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "specification"@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.linkedSchemas>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.linkedSchemas>
];
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ApplicationProfile>.

<https://techlib.cz/profile/ccmm/Distribution-DataService.documentation> dsv:domain <https://techlib.cz/profile/ccmm/Distribution-DataService>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.documentation>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.documentation>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Distribution.documentation>
];
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Documentation>.

<https://techlib.cz/profile/ccmm/File> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "File"@en;
    skos:definition "Downloadable file in given format."@en;
    dsv:profileOf <https://techlib.cz/profile/ccmm/Resource>;
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/File.label> dsv:domain <https://techlib.cz/profile/ccmm/File>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "File label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/MediaType> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#MediaType>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#MediaType>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#MediaType>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/MediaType.label> dsv:domain <https://techlib.cz/profile/ccmm/MediaType>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Media type label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/DataService> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#DataService>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#DataService>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#DataService>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/DataService.endpointUrl> dsv:domain <https://techlib.cz/profile/ccmm/DataService>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#DataService.endpointUrl>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#DataService.endpointUrl>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#DataService.endpointUrl>
];
    dsv:cardinality cardinality:1n;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Resource>.

<https://techlib.cz/profile/ccmm/DataService.label> dsv:domain <https://techlib.cz/profile/ccmm/DataService>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Data service label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Documentation> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Documentation"@en;
    skos:definition "A textual resource intended for human consumption that contains information about the data service."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Document>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/Documentation.label> dsv:domain <https://techlib.cz/profile/ccmm/Documentation>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Documentation label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/SubjectScheme> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Subject scheme"@en;
    skos:definition "Collection of subjects (keywords, concepts, classification codes) curated by a single source."@en;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/ccmm/SubjectScheme>.

<https://techlib.cz/profile/ccmm/SubjectScheme.label> dsv:domain <https://techlib.cz/profile/ccmm/SubjectScheme>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Subject scheme label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Location> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "Spatial region, named place or geometry representation of a place related to the dataset. "@en;
    vann:usageNote "Location is represented according to the Open Formal Standard for Spatial Data. Locations are represented as a reference to the named place, geographic region or specific geometry according to the standardized geometry object representation. \nEvery Location instance must have at least one of the following attributes/relations: bbox, location name, geometry and/or related object identifier."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Location>
];
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/datacite/Geolocation>.

<https://techlib.cz/profile/ccmm/Location.relationToDataset> dsv:domain <https://techlib.cz/profile/ccmm/Location>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Relation type"@en;
    skos:definition "Type of the relation between Dataset and related resource."@en;
    vann:usageNote "Use value from controlled list."@en;
    dsv:cardinality cardinality:11;
    dsv:property <https://techlib.cz/vocabulary/ccmm/relationType>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/Location.hasRelatedObjectIdentifier> dsv:domain <https://techlib.cz/profile/ccmm/Location>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "has related resource"@en;
    vann:usageNote "The relation is provided by the idnetifer of the related object."@en;
    dsv:property <https://techlib.cz/vocabulary/ccmm/hasRelatedResource>;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Resource>.

<https://techlib.cz/profile/ccmm/Location.name> dsv:domain <https://techlib.cz/profile/ccmm/Location>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Location name"@en;
    skos:definition "Name of the spatial location."@en;
    dsv:cardinality cardinality:0n;
    dsv:property dct:title;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <https://ofn.gov.cz/zdroj/základní-datové-typy/2020-07-01/text>.

<https://techlib.cz/profile/ccmm/Location.bbox> dsv:domain <https://techlib.cz/profile/ccmm/Location>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Bounding box"@en;
    skos:definition "Bounding box of the location geometry."@en;
    dsv:cardinality cardinality:0n;
    dsv:property <http://www.w3.org/ns/dcat#bbox>;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdfs:Literal.

<https://techlib.cz/profile/ccmm/Location.geometry> dsv:domain <https://techlib.cz/profile/ccmm/Location>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Location.geometry>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Location.geometry>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Location.geometry>
];
    dsv:cardinality cardinality:01;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/Geometry>.

<https://techlib.cz/profile/ccmm/Geometry> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:definition "The Geometry class provides the means to identify a location as a point, line, polygon, etc. expressed using coordinates in some coordinate reference system."@en;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Geometry>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Geometry>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/Geometry.label> dsv:domain <https://techlib.cz/profile/ccmm/Geometry>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Label"@en;
    skos:definition "Geometry label"@en;
    dsv:cardinality cardinality:0n;
    dsv:property rdfs:label;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Identifier> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <http://www.w3.org/ns/adms#Identifier>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <http://www.w3.org/ns/adms#Identifier>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <:undefined-identifier:>
];
    a dsv:ClassProfile;
    dsv:class <http://www.w3.org/ns/adms#Identifier>.

<https://techlib.cz/profile/ccmm/Identifier.value> dsv:domain <https://techlib.cz/profile/ccmm/Identifier>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Value"@en;
    skos:definition "Value of the identifer within the given identifier scheme."@en;
    dsv:cardinality cardinality:11;
    dsv:property skos:notation;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdfs:Literal.

<https://techlib.cz/profile/ccmm/Identifier.identifierScheme> dsv:domain <https://techlib.cz/profile/ccmm/Identifier>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Identifier scheme"@en;
    skos:definition "Scheme URI of the identifier."@en;
    vann:usageNote "For DOI, ROR etc. use the scheme uri, i.e. https://doi.org/. For others use base URI (whole URI except the notation part)."@en;
    dsv:cardinality cardinality:11;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#anyURI>.

<https://techlib.cz/profile/ccmm/AlternateTitle> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Alternate title"@en;
    skos:definition "Alternate title of the resource."@en;
    a dsv:ClassProfile;
    dsv:class <https://techlib.cz/vocabulary/ccmm/AlternateTitle>.

<https://techlib.cz/profile/ccmm/AlternateTitle.type> dsv:domain <https://techlib.cz/profile/ccmm/AlternateTitle>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Type"@en;
    skos:definition "Type of alternate title."@en;
    vann:usageNote "Use value from controlled list with values:\n- AlternativeTitle\n- Subtitle\n- TranslatedTitle\n\nFor other leave empty"@en;
    dsv:cardinality cardinality:01;
    dsv:property dct:type;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#string>.

<https://techlib.cz/profile/ccmm/AlternateTitle.title> dsv:domain <https://techlib.cz/profile/ccmm/AlternateTitle>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Title"@en;
    skos:definition "Alternate title name."@en;
    dsv:cardinality cardinality:11;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange rdf:langString.

<https://techlib.cz/profile/ccmm/Checksum> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Checksum>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Checksum>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Checksum>
];
    a dsv:ClassProfile.

<https://techlib.cz/profile/ccmm/Checksum.checksumValue> dsv:domain <https://techlib.cz/profile/ccmm/Checksum>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    skos:prefLabel "Checksum value"@en;
    skos:definition "A lower case hexadecimal encoded digest value produced using a specific algorithm. "@en;
    dsv:cardinality cardinality:11;
    a dsv:DatatypePropertyProfile;
    dsv:datatypePropertyRange <http://www.w3.org/2001/XMLSchema#hexBinary>.

<https://techlib.cz/profile/ccmm/Checksum.algorithm> dsv:domain <https://techlib.cz/profile/ccmm/Checksum>;
    dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Checksum.algorithm>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Checksum.algorithm>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#Checksum.algorithm>
];
    dsv:cardinality cardinality:11;
    a dsv:ObjectPropertyProfile;
    dsv:objectPropertyRange <https://techlib.cz/profile/ccmm/ChecksumAlgorithm>.

<https://techlib.cz/profile/ccmm/ChecksumAlgorithm> dct:isPartOf <https://techlib.cz/profile/ccmm/applicationProfileConceptualModel>;
    a dsv:Profile;
    dsv:reusesPropertyValue [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:prefLabel;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#ChecksumAlgorithm>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty skos:definition;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#ChecksumAlgorithm>
], [
  a dsv:PropertyValueReuse;
  dsv:reusedProperty vann:usageNote;
  dsv:reusedFromResource <https://mff-uk.github.io/specifications/dcat-ap/#ChecksumAlgorithm>
];
    a dsv:ClassProfile.
```

### Answer

This is a RDF (Resource Description Framework) data in Turtle format, which is used to describe and represent data on the web. It appears to be a vocabulary or ontology for describing digital resources,
such as files and datasets.

Here's a breakdown of the different parts:

* `@prefix` directives define shortcuts for namespaces, making it easier to write URIs.
* `dct:isPartOf` properties indicate that a resource is part of a larger collection or concept.
* `dsv:Profile` instances represent concepts or classes in the vocabulary, such as "Checksum" or "AlternateTitle".
* `skos:prefLabel`, `skos:definition`, and `vann:usageNote` properties provide human-readable labels, definitions, and usage notes for each concept.
* `dsv:reusesPropertyValue` instances indicate that a property value is being reused from another resource.
* `dsv:cardinality` properties specify the cardinality of a property, such as "1" (one) or "0..1" (zero or one).
* `dsv:objectPropertyRange` and `dsv:datatypePropertyRange` properties specify the range of values for object and datatype properties, respectively.

Some examples of concepts defined in this vocabulary include:

* `Checksum`: a class representing a checksum value and algorithm.
* `AlternateTitle`: a class representing an alternate title for a resource.
* `ChecksumAlgorithm`: a class representing a specific checksum algorithm.

Overall, this vocabulary appears to be designed for describing digital resources and their associated metadata, such as checksums, titles, and algorithms.

To write similar code, you would need to define your own concepts, properties, and relationships using the same RDF/Turtle syntax. You can use tools like Protege or Turtle editors to create and edit your
own vocabularies.

Here is an example of how you might add a new concept to this vocabulary:
```turtle
@prefix dsv: <https://example.org/vocab#> .
@prefix skos: <http://www.w3.org/2004/02/skos/core#> .

dsv:NewConcept a dsv:Profile ;
    skos:prefLabel "New Concept" ;
    skos:definition "A new concept in the vocabulary" ;
    vann:usageNote "Use this concept to describe..." .
```
This code defines a new concept called "NewConcept" with a label, definition, and usage note. You can then use this concept in your own data by referencing its URI
(`https://example.org/vocab#NewConcept`).