<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">
  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

  <xsl:template match="wix:Wix">
    <xsl:copy>
      <xsl:processing-instruction name="include">
        <xsl:text>include.wxi</xsl:text>
      </xsl:processing-instruction>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="wix:File[(@Source = '$(var.DepsCommon)\Nemeio.exe')]">
    <xsl:copy>
      <xsl:attribute name="Id">Nemeio.exe</xsl:attribute>
      <xsl:copy-of select="@*[name()!='Id']"/>
      <xsl:apply-templates />
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>