<?xml version="1.0" encoding="UTF-8"?>
<drawing version="7">
    <attr value="artix7" name="DeviceFamilyName">
        <trait delete="all:0" />
        <trait editname="all:0" />
        <trait edittrait="all:0" />
    </attr>
    <netlist>
        <signal name="funcao_tara" />
        <signal name="peso_kg(15:0)" />
        <signal name="XLXN_20(8:0)" />
        <signal name="calculo_euros(15:0)" />
        <signal name="BCDkg_decimal(3:0)" />
        <signal name="BCDkg_fracionario(11:0)" />
        <signal name="BCDeuros_decimal(7:0)" />
        <signal name="BCDeuros_fracionario(7:0)" />
        <signal name="emissao_talao" />
        <signal name="XLXN_61(10:0)" />
        <signal name="clk" />
        <signal name="rst" />
        <signal name="valor_taxa(4:0)" />
        <signal name="XLXN_76(10:0)" />
        <signal name="XLXN_79(10:0)" />
        <signal name="fim_compra" />
        <signal name="taxa" />
        <signal name="XLXN_84(10:0)" />
        <signal name="produto(1:0)" />
        <signal name="peso_banana(10:0)" />
        <signal name="preco_banana(8:0)" />
        <signal name="preco_maracuja(8:0)" />
        <signal name="peso_maracuja(10:0)" />
        <signal name="peso_tangerina(10:0)" />
        <signal name="preco_tangerina(8:0)" />
        <port polarity="Input" name="funcao_tara" />
        <port polarity="Output" name="BCDkg_decimal(3:0)" />
        <port polarity="Output" name="BCDkg_fracionario(11:0)" />
        <port polarity="Output" name="BCDeuros_decimal(7:0)" />
        <port polarity="Output" name="BCDeuros_fracionario(7:0)" />
        <port polarity="Output" name="emissao_talao" />
        <port polarity="Input" name="clk" />
        <port polarity="Input" name="rst" />
        <port polarity="Output" name="valor_taxa(4:0)" />
        <port polarity="Input" name="fim_compra" />
        <port polarity="Input" name="taxa" />
        <port polarity="Input" name="produto(1:0)" />
        <port polarity="Input" name="peso_banana(10:0)" />
        <port polarity="Input" name="preco_banana(8:0)" />
        <port polarity="Input" name="preco_maracuja(8:0)" />
        <port polarity="Input" name="peso_maracuja(10:0)" />
        <port polarity="Input" name="peso_tangerina(10:0)" />
        <port polarity="Input" name="preco_tangerina(8:0)" />
        <blockdef name="separador_BCD_4_12">
            <timestamp>2023-11-10T17:28:17</timestamp>
            <rect width="416" x="64" y="-128" height="128" />
            <rect width="64" x="0" y="-108" height="24" />
            <line x2="0" y1="-96" y2="-96" x1="64" />
            <rect width="64" x="480" y="-108" height="24" />
            <line x2="544" y1="-96" y2="-96" x1="480" />
            <rect width="64" x="480" y="-44" height="24" />
            <line x2="544" y1="-32" y2="-32" x1="480" />
        </blockdef>
        <blockdef name="separador_BCD_8_8">
            <timestamp>2023-11-10T17:35:55</timestamp>
            <rect width="400" x="64" y="-128" height="128" />
            <rect width="64" x="0" y="-108" height="24" />
            <line x2="0" y1="-96" y2="-96" x1="64" />
            <rect width="64" x="464" y="-108" height="24" />
            <line x2="528" y1="-96" y2="-96" x1="464" />
            <rect width="64" x="464" y="-44" height="24" />
            <line x2="528" y1="-32" y2="-32" x1="464" />
        </blockdef>
        <blockdef name="bin2bcd_11_16">
            <timestamp>2023-12-22T1:41:29</timestamp>
            <rect width="256" x="64" y="-64" height="64" />
            <rect width="64" x="0" y="-44" height="24" />
            <line x2="0" y1="-32" y2="-32" x1="64" />
            <rect width="64" x="320" y="-44" height="24" />
            <line x2="384" y1="-32" y2="-32" x1="320" />
        </blockdef>
        <blockdef name="multiplicador_11_9_bin_11">
            <timestamp>2023-11-10T18:32:46</timestamp>
            <rect width="368" x="64" y="-128" height="128" />
            <rect width="64" x="0" y="-108" height="24" />
            <line x2="0" y1="-96" y2="-96" x1="64" />
            <rect width="64" x="0" y="-44" height="24" />
            <line x2="0" y1="-32" y2="-32" x1="64" />
            <rect width="64" x="432" y="-108" height="24" />
            <line x2="496" y1="-96" y2="-96" x1="432" />
        </blockdef>
        <blockdef name="somador_preco_peso">
            <timestamp>2023-12-22T1:27:55</timestamp>
            <rect width="400" x="64" y="-384" height="384" />
            <line x2="0" y1="-352" y2="-352" x1="64" />
            <line x2="0" y1="-288" y2="-288" x1="64" />
            <line x2="0" y1="-224" y2="-224" x1="64" />
            <line x2="0" y1="-160" y2="-160" x1="64" />
            <rect width="64" x="0" y="-108" height="24" />
            <line x2="0" y1="-96" y2="-96" x1="64" />
            <rect width="64" x="0" y="-44" height="24" />
            <line x2="0" y1="-32" y2="-32" x1="64" />
            <line x2="528" y1="-352" y2="-352" x1="464" />
            <rect width="64" x="464" y="-268" height="24" />
            <line x2="528" y1="-256" y2="-256" x1="464" />
            <rect width="64" x="464" y="-172" height="24" />
            <line x2="528" y1="-160" y2="-160" x1="464" />
            <rect width="64" x="464" y="-76" height="24" />
            <line x2="528" y1="-64" y2="-64" x1="464" />
        </blockdef>
        <blockdef name="filtragem_peso">
            <timestamp>2023-12-23T13:20:17</timestamp>
            <rect width="64" x="0" y="20" height="24" />
            <line x2="0" y1="32" y2="32" x1="64" />
            <rect width="64" x="0" y="84" height="24" />
            <line x2="0" y1="96" y2="96" x1="64" />
            <rect width="64" x="0" y="148" height="24" />
            <line x2="0" y1="160" y2="160" x1="64" />
            <rect width="64" x="0" y="212" height="24" />
            <line x2="0" y1="224" y2="224" x1="64" />
            <line x2="0" y1="-96" y2="-96" x1="64" />
            <rect width="64" x="400" y="-108" height="24" />
            <line x2="464" y1="-96" y2="-96" x1="400" />
            <rect width="336" x="64" y="-128" height="384" />
        </blockdef>
        <blockdef name="filtragem_preco">
            <timestamp>2023-12-23T13:20:29</timestamp>
            <rect width="64" x="0" y="20" height="24" />
            <line x2="0" y1="32" y2="32" x1="64" />
            <rect width="64" x="0" y="84" height="24" />
            <line x2="0" y1="96" y2="96" x1="64" />
            <rect width="64" x="0" y="148" height="24" />
            <line x2="0" y1="160" y2="160" x1="64" />
            <rect width="64" x="0" y="212" height="24" />
            <line x2="0" y1="224" y2="224" x1="64" />
            <rect width="64" x="384" y="-44" height="24" />
            <line x2="448" y1="-32" y2="-32" x1="384" />
            <rect width="320" x="64" y="-64" height="320" />
        </blockdef>
        <block symbolname="separador_BCD_4_12" name="XLXI_13">
            <blockpin signalname="peso_kg(15:0)" name="BCD_total(15:0)" />
            <blockpin signalname="BCDkg_decimal(3:0)" name="BCD_inteiro(3:0)" />
            <blockpin signalname="BCDkg_fracionario(11:0)" name="BCD_fracionario(11:0)" />
        </block>
        <block symbolname="separador_BCD_8_8" name="XLXI_14">
            <blockpin signalname="calculo_euros(15:0)" name="BCD_total(15:0)" />
            <blockpin signalname="BCDeuros_decimal(7:0)" name="BCD_inteiro(7:0)" />
            <blockpin signalname="BCDeuros_fracionario(7:0)" name="BCD_fracionario(7:0)" />
        </block>
        <block symbolname="bin2bcd_11_16" name="XLXI_15">
            <blockpin signalname="XLXN_79(10:0)" name="bin(10:0)" />
            <blockpin signalname="peso_kg(15:0)" name="bcd(15:0)" />
        </block>
        <block symbolname="bin2bcd_11_16" name="XLXI_16">
            <blockpin signalname="XLXN_76(10:0)" name="bin(10:0)" />
            <blockpin signalname="calculo_euros(15:0)" name="bcd(15:0)" />
        </block>
        <block symbolname="multiplicador_11_9_bin_11" name="XLXI_17">
            <blockpin signalname="XLXN_84(10:0)" name="entrada_11b(10:0)" />
            <blockpin signalname="XLXN_20(8:0)" name="entrada_9b(8:0)" />
            <blockpin signalname="XLXN_61(10:0)" name="saida_11b(10:0)" />
        </block>
        <block symbolname="somador_preco_peso" name="XLXI_25">
            <blockpin signalname="clk" name="clk" />
            <blockpin signalname="rst" name="rst" />
            <blockpin signalname="fim_compra" name="fim_compra" />
            <blockpin signalname="taxa" name="taxa" />
            <blockpin signalname="XLXN_61(10:0)" name="preco_produto(10:0)" />
            <blockpin signalname="XLXN_84(10:0)" name="peso_produto(10:0)" />
            <blockpin signalname="emissao_talao" name="emissao_talao" />
            <blockpin signalname="XLXN_76(10:0)" name="soma_final(10:0)" />
            <blockpin signalname="XLXN_79(10:0)" name="soma_peso(10:0)" />
            <blockpin signalname="valor_taxa(4:0)" name="valor_taxa(4:0)" />
        </block>
        <block symbolname="filtragem_peso" name="XLXI_26">
            <blockpin signalname="funcao_tara" name="tara" />
            <blockpin signalname="XLXN_84(10:0)" name="peso_liq(10:0)" />
            <blockpin signalname="produto(1:0)" name="produto(1:0)" />
            <blockpin signalname="peso_banana(10:0)" name="peso_banana(10:0)" />
            <blockpin signalname="peso_maracuja(10:0)" name="peso_maracuja(10:0)" />
            <blockpin signalname="peso_tangerina(10:0)" name="peso_tangerina(10:0)" />
        </block>
        <block symbolname="filtragem_preco" name="XLXI_27">
            <blockpin signalname="XLXN_20(8:0)" name="preco_fil(8:0)" />
            <blockpin signalname="produto(1:0)" name="produto(1:0)" />
            <blockpin signalname="preco_banana(8:0)" name="preco_banana(8:0)" />
            <blockpin signalname="preco_maracuja(8:0)" name="preco_maracuja(8:0)" />
            <blockpin signalname="preco_tangerina(8:0)" name="preco_tangerina(8:0)" />
        </block>
    </netlist>
    <sheet sheetnum="1" width="3520" height="2720">
        <branch name="funcao_tara">
            <wire x2="448" y1="576" y2="576" x1="272" />
        </branch>
        <branch name="XLXN_20(8:0)">
            <wire x2="1184" y1="1056" y2="1056" x1="912" />
            <wire x2="1200" y1="1024" y2="1024" x1="1184" />
            <wire x2="1184" y1="1024" y2="1056" x1="1184" />
        </branch>
        <branch name="calculo_euros(15:0)">
            <wire x2="2352" y1="960" y2="960" x1="2240" />
        </branch>
        <branch name="BCDkg_fracionario(11:0)">
            <wire x2="3024" y1="640" y2="640" x1="2224" />
        </branch>
        <branch name="BCDkg_decimal(3:0)">
            <wire x2="3024" y1="576" y2="576" x1="2224" />
        </branch>
        <branch name="peso_kg(15:0)">
            <wire x2="1680" y1="576" y2="576" x1="1488" />
        </branch>
        <branch name="BCDeuros_decimal(7:0)">
            <wire x2="3024" y1="960" y2="960" x1="2880" />
        </branch>
        <branch name="BCDeuros_fracionario(7:0)">
            <wire x2="3024" y1="1024" y2="1024" x1="2880" />
        </branch>
        <instance x="1680" y="672" name="XLXI_13" orien="R0">
        </instance>
        <iomarker fontsize="28" x="272" y="576" name="funcao_tara" orien="R180" />
        <instance x="1200" y="1056" name="XLXI_17" orien="R0">
        </instance>
        <instance x="1856" y="992" name="XLXI_16" orien="R0">
        </instance>
        <instance x="2352" y="1056" name="XLXI_14" orien="R0">
        </instance>
        <iomarker fontsize="28" x="3024" y="960" name="BCDeuros_decimal(7:0)" orien="R0" />
        <iomarker fontsize="28" x="3024" y="1024" name="BCDeuros_fracionario(7:0)" orien="R0" />
        <iomarker fontsize="28" x="3024" y="640" name="BCDkg_fracionario(11:0)" orien="R0" />
        <iomarker fontsize="28" x="3024" y="576" name="BCDkg_decimal(3:0)" orien="R0" />
        <branch name="XLXN_61(10:0)">
            <wire x2="1712" y1="1120" y2="1120" x1="1424" />
            <wire x2="1424" y1="1120" y2="1520" x1="1424" />
            <wire x2="1648" y1="1520" y2="1520" x1="1424" />
            <wire x2="1712" y1="960" y2="960" x1="1696" />
            <wire x2="1712" y1="960" y2="1120" x1="1712" />
        </branch>
        <branch name="clk">
            <wire x2="1648" y1="1264" y2="1264" x1="1632" />
        </branch>
        <branch name="rst">
            <wire x2="1648" y1="1328" y2="1328" x1="1632" />
        </branch>
        <instance x="1648" y="1616" name="XLXI_25" orien="R0">
        </instance>
        <branch name="emissao_talao">
            <wire x2="2208" y1="1264" y2="1264" x1="2176" />
        </branch>
        <iomarker fontsize="28" x="2208" y="1264" name="emissao_talao" orien="R0" />
        <branch name="valor_taxa(4:0)">
            <wire x2="2240" y1="1552" y2="1552" x1="2176" />
        </branch>
        <iomarker fontsize="28" x="2240" y="1552" name="valor_taxa(4:0)" orien="R0" />
        <branch name="XLXN_76(10:0)">
            <wire x2="1792" y1="960" y2="1120" x1="1792" />
            <wire x2="2464" y1="1120" y2="1120" x1="1792" />
            <wire x2="2464" y1="1120" y2="1360" x1="2464" />
            <wire x2="1856" y1="960" y2="960" x1="1792" />
            <wire x2="2464" y1="1360" y2="1360" x1="2176" />
        </branch>
        <branch name="fim_compra">
            <wire x2="1648" y1="1392" y2="1392" x1="1632" />
        </branch>
        <iomarker fontsize="28" x="1632" y="1392" name="fim_compra" orien="R180" />
        <branch name="taxa">
            <wire x2="1648" y1="1456" y2="1456" x1="1632" />
        </branch>
        <iomarker fontsize="28" x="1632" y="1456" name="taxa" orien="R180" />
        <iomarker fontsize="28" x="1632" y="1328" name="rst" orien="R180" />
        <iomarker fontsize="28" x="1632" y="1264" name="clk" orien="R180" />
        <branch name="XLXN_84(10:0)">
            <wire x2="1056" y1="576" y2="576" x1="912" />
            <wire x2="1056" y1="576" y2="960" x1="1056" />
            <wire x2="1056" y1="960" y2="1584" x1="1056" />
            <wire x2="1648" y1="1584" y2="1584" x1="1056" />
            <wire x2="1200" y1="960" y2="960" x1="1056" />
        </branch>
        <branch name="XLXN_79(10:0)">
            <wire x2="3440" y1="352" y2="352" x1="1088" />
            <wire x2="3440" y1="352" y2="1456" x1="3440" />
            <wire x2="1088" y1="352" y2="576" x1="1088" />
            <wire x2="1104" y1="576" y2="576" x1="1088" />
            <wire x2="3440" y1="1456" y2="1456" x1="2176" />
        </branch>
        <instance x="1104" y="608" name="XLXI_15" orien="R0">
        </instance>
        <instance x="448" y="672" name="XLXI_26" orien="R0">
        </instance>
        <instance x="464" y="1088" name="XLXI_27" orien="R0">
        </instance>
        <branch name="produto(1:0)">
            <wire x2="368" y1="704" y2="704" x1="272" />
            <wire x2="432" y1="704" y2="704" x1="368" />
            <wire x2="448" y1="704" y2="704" x1="432" />
            <wire x2="368" y1="704" y2="1120" x1="368" />
            <wire x2="464" y1="1120" y2="1120" x1="368" />
        </branch>
        <iomarker fontsize="28" x="272" y="704" name="produto(1:0)" orien="R180" />
        <branch name="peso_banana(10:0)">
            <wire x2="448" y1="768" y2="768" x1="416" />
        </branch>
        <iomarker fontsize="28" x="416" y="768" name="peso_banana(10:0)" orien="R180" />
        <branch name="peso_maracuja(10:0)">
            <wire x2="448" y1="832" y2="832" x1="416" />
        </branch>
        <iomarker fontsize="28" x="416" y="832" name="peso_maracuja(10:0)" orien="R180" />
        <branch name="peso_tangerina(10:0)">
            <wire x2="448" y1="896" y2="896" x1="416" />
        </branch>
        <iomarker fontsize="28" x="416" y="896" name="peso_tangerina(10:0)" orien="R180" />
        <branch name="preco_banana(8:0)">
            <wire x2="464" y1="1184" y2="1184" x1="432" />
        </branch>
        <iomarker fontsize="28" x="432" y="1184" name="preco_banana(8:0)" orien="R180" />
        <branch name="preco_maracuja(8:0)">
            <wire x2="464" y1="1248" y2="1248" x1="432" />
        </branch>
        <iomarker fontsize="28" x="432" y="1248" name="preco_maracuja(8:0)" orien="R180" />
        <branch name="preco_tangerina(8:0)">
            <wire x2="464" y1="1312" y2="1312" x1="432" />
        </branch>
        <iomarker fontsize="28" x="432" y="1312" name="preco_tangerina(8:0)" orien="R180" />
    </sheet>
</drawing>