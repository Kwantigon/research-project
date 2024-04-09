<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" version="2.0" xmlns:c="https://schemas.dataspecer.com/xsd/core/" xmlns:ns0="https://slovník.gov.cz/generický/věda-a-výzkum/pojem/" xmlns:ns1="https://slovník.gov.cz/datový/školství/pojem/">
  <xsl:output method="xml" version="1.0" encoding="utf-8" media-type="application/rdf+xml" indent="yes"/>
  <xsl:template match="/dissertation_topic">
    <rdf:RDF>
      <xsl:variable name="result">
        <xsl:sequence>
          <xsl:call-template name="_https_003a_002f_002fofn.gov.cz_002fclass_002f1712492237515-b215-7b46-8d37"/>
        </xsl:sequence>
      </xsl:variable>
      <xsl:for-each select="$result">
        <xsl:copy>
          <xsl:call-template name="remove-top"/>
        </xsl:copy>
      </xsl:for-each>
      <xsl:for-each select="$result//top-level/node()">
        <xsl:copy>
          <xsl:call-template name="remove-top"/>
        </xsl:copy>
      </xsl:for-each>
    </rdf:RDF>
  </xsl:template>
  <xsl:template match="@xml:lang">
    <xsl:copy-of select="."/>
  </xsl:template>
  <xsl:template name="remove-top">
    <xsl:for-each select="@*">
      <xsl:copy/>
    </xsl:for-each>
    <xsl:for-each select="node()[not(. instance of element(top-level))]">
      <xsl:copy>
        <xsl:call-template name="remove-top"/>
      </xsl:copy>
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="_https_003a_002f_002fofn.gov.cz_002fclass_002f1712492237515-b215-7b46-8d37">
    <xsl:param name="arc" select="()"/>
    <xsl:param name="no_iri" select="false()"/>
    <rdf:Description>
      <xsl:apply-templates select="@*"/>
      <xsl:variable name="id">
        <id>
          <xsl:choose>
            <xsl:when test="c:iri and not($no_iri)">
              <xsl:attribute name="rdf:about">
                <xsl:value-of select="c:iri"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="rdf:nodeID">
                <xsl:value-of select="generate-id()"/>
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
        </id>
      </xsl:variable>
      <xsl:copy-of select="$id//@*"/>
      <rdf:type rdf:resource="https://slovník.gov.cz/generický/věda-a-výzkum/pojem/téma-dizertační-práce"/>
      <xsl:copy-of select="$arc"/>
      <xsl:for-each select="currently_offered">
        <ns0:aktuálně-nabízeno rdf:datatype="http://www.w3.org/2001/XMLSchema#boolean">
          <xsl:apply-templates select="@*"/>
          <xsl:value-of select="."/>
        </ns0:aktuálně-nabízeno>
      </xsl:for-each>
      <xsl:for-each select="recommended_workplaces_in_general">
        <ns0:doporučovaná-pracoviště-obecně>
          <xsl:apply-templates select="@*"/>
          <xsl:value-of select="."/>
        </ns0:doporučovaná-pracoviště-obecně>
      </xsl:for-each>
      <xsl:for-each select="the_context_of_the_dissertation_topic">
        <ns0:kontext-tématu-dizertační-práce>
          <xsl:apply-templates select="@*"/>
          <xsl:value-of select="."/>
        </ns0:kontext-tématu-dizertační-práce>
      </xsl:for-each>
      <xsl:for-each select="has_a_study_program">
        <ns0:má-studijní-program>
          <xsl:call-template name="_https_003a_002f_002fofn.gov.cz_002fclass_002f1712492266135-9afb-0bff-a173"/>
        </ns0:má-studijní-program>
      </xsl:for-each>
      <xsl:for-each select="has_a_specific_recommended_workplace">
        <ns0:má-konkrétní-doporučované-pracoviště>
          <xsl:call-template name="_https_003a_002f_002fofn.gov.cz_002fclass_002f1712492266336-6d1b-2eea-b100"/>
        </ns0:má-konkrétní-doporučované-pracoviště>
      </xsl:for-each>
    </rdf:Description>
  </xsl:template>
  <xsl:template name="_https_003a_002f_002fofn.gov.cz_002fclass_002f1712492266135-9afb-0bff-a173">
    <xsl:param name="arc" select="()"/>
    <xsl:param name="no_iri" select="false()"/>
    <rdf:Description>
      <xsl:apply-templates select="@*"/>
      <xsl:variable name="id">
        <id>
          <xsl:choose>
            <xsl:when test="c:iri and not($no_iri)">
              <xsl:attribute name="rdf:about">
                <xsl:value-of select="c:iri"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="rdf:nodeID">
                <xsl:value-of select="generate-id()"/>
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
        </id>
      </xsl:variable>
      <xsl:copy-of select="$id//@*"/>
      <rdf:type rdf:resource="https://slovník.gov.cz/datový/školství/pojem/studijní-program"/>
      <xsl:copy-of select="$arc"/>
      <xsl:for-each select="code">
        <ns1:kód rdf:datatype="http://www.w3.org/2001/XMLSchema#string">
          <xsl:apply-templates select="@*"/>
          <xsl:value-of select="."/>
        </ns1:kód>
      </xsl:for-each>
    </rdf:Description>
  </xsl:template>
  <xsl:template name="_https_003a_002f_002fofn.gov.cz_002fclass_002f1712492266336-6d1b-2eea-b100">
    <xsl:param name="arc" select="()"/>
    <xsl:param name="no_iri" select="false()"/>
    <rdf:Description>
      <xsl:apply-templates select="@*"/>
      <xsl:variable name="id">
        <id>
          <xsl:choose>
            <xsl:when test="c:iri and not($no_iri)">
              <xsl:attribute name="rdf:about">
                <xsl:value-of select="c:iri"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="rdf:nodeID">
                <xsl:value-of select="generate-id()"/>
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
        </id>
      </xsl:variable>
      <xsl:copy-of select="$id//@*"/>
      <rdf:type rdf:resource="https://slovník.gov.cz/generický/věda-a-výzkum/pojem/výzkumné-pracoviště"/>
      <xsl:copy-of select="$arc"/>
      <xsl:for-each select="orjk">
        <ns0:orjk>
          <xsl:apply-templates select="@*"/>
          <xsl:value-of select="."/>
        </ns0:orjk>
      </xsl:for-each>
    </rdf:Description>
  </xsl:template>
  <xsl:template match="@*|*"/>
</xsl:stylesheet>
