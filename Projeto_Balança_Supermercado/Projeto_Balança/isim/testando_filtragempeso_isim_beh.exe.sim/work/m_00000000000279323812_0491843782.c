/**********************************************************************/
/*   ____  ____                                                       */
/*  /   /\/   /                                                       */
/* /___/  \  /                                                        */
/* \   \   \/                                                       */
/*  \   \        Copyright (c) 2003-2009 Xilinx, Inc.                */
/*  /   /          All Right Reserved.                                 */
/* /---/   /\                                                         */
/* \   \  /  \                                                      */
/*  \___\/\___\                                                    */
/***********************************************************************/

/* This file is designed for use with ISim build 0x7708f090 */

#define XSI_HIDE_SYMBOL_SPEC true
#include "xsi.h"
#include <memory.h>
#ifdef __GNUC__
#include <stdlib.h>
#else
#include <malloc.h>
#define alloca _alloca
#endif
static const char *ng0 = "C:/Users/joaoc/Desktop/PROJETOS/Projeto_F1v2/filtragem_peso.v";
static unsigned int ng1[] = {1U, 0U};
static unsigned int ng2[] = {2U, 0U};
static unsigned int ng3[] = {3U, 0U};
static int ng4[] = {0, 0};
static unsigned int ng5[] = {60U, 0U};
static unsigned int ng6[] = {2000U, 0U};
static unsigned int ng7[] = {0U, 0U};
static unsigned int ng8[] = {1940U, 0U};



static void Always_33_0(char *t0)
{
    char *t1;
    char *t2;
    char *t3;
    char *t4;
    char *t5;
    int t6;
    char *t7;
    char *t8;

LAB0:    t1 = (t0 + 3168U);
    t2 = *((char **)t1);
    if (t2 == 0)
        goto LAB2;

LAB3:    goto *t2;

LAB2:    xsi_set_current_line(33, ng0);
    t2 = (t0 + 3736);
    *((int *)t2) = 1;
    t3 = (t0 + 3200);
    *((char **)t3) = t2;
    *((char **)t1) = &&LAB4;

LAB1:    return;
LAB4:    xsi_set_current_line(33, ng0);

LAB5:    xsi_set_current_line(34, ng0);
    t4 = (t0 + 1048U);
    t5 = *((char **)t4);

LAB6:    t4 = ((char*)((ng1)));
    t6 = xsi_vlog_unsigned_case_compare(t5, 2, t4, 2);
    if (t6 == 1)
        goto LAB7;

LAB8:    t2 = ((char*)((ng2)));
    t6 = xsi_vlog_unsigned_case_compare(t5, 2, t2, 2);
    if (t6 == 1)
        goto LAB9;

LAB10:    t2 = ((char*)((ng3)));
    t6 = xsi_vlog_unsigned_case_compare(t5, 2, t2, 2);
    if (t6 == 1)
        goto LAB11;

LAB12:
LAB14:
LAB13:    xsi_set_current_line(38, ng0);
    t2 = ((char*)((ng4)));
    t3 = (t0 + 2248);
    xsi_vlogvar_assign_value(t3, t2, 0, 0, 11);

LAB15:    goto LAB2;

LAB7:    xsi_set_current_line(35, ng0);
    t7 = (t0 + 1208U);
    t8 = *((char **)t7);
    t7 = (t0 + 2248);
    xsi_vlogvar_assign_value(t7, t8, 0, 0, 11);
    goto LAB15;

LAB9:    xsi_set_current_line(36, ng0);
    t3 = (t0 + 1368U);
    t4 = *((char **)t3);
    t3 = (t0 + 2248);
    xsi_vlogvar_assign_value(t3, t4, 0, 0, 11);
    goto LAB15;

LAB11:    xsi_set_current_line(37, ng0);
    t3 = (t0 + 1528U);
    t4 = *((char **)t3);
    t3 = (t0 + 2248);
    xsi_vlogvar_assign_value(t3, t4, 0, 0, 11);
    goto LAB15;

}

static void Always_42_1(char *t0)
{
    char t6[8];
    char t21[8];
    char t25[8];
    char t41[8];
    char t45[8];
    char t53[8];
    char t85[8];
    char t93[8];
    char t135[8];
    char *t1;
    char *t2;
    char *t3;
    char *t4;
    char *t5;
    unsigned int t7;
    unsigned int t8;
    unsigned int t9;
    unsigned int t10;
    unsigned int t11;
    char *t12;
    char *t13;
    unsigned int t14;
    unsigned int t15;
    unsigned int t16;
    char *t17;
    char *t18;
    char *t19;
    char *t20;
    char *t22;
    char *t23;
    char *t24;
    char *t26;
    unsigned int t27;
    unsigned int t28;
    unsigned int t29;
    unsigned int t30;
    unsigned int t31;
    char *t32;
    char *t33;
    unsigned int t34;
    unsigned int t35;
    unsigned int t36;
    char *t37;
    char *t38;
    char *t39;
    char *t40;
    char *t42;
    char *t43;
    char *t44;
    char *t46;
    unsigned int t47;
    unsigned int t48;
    unsigned int t49;
    unsigned int t50;
    unsigned int t51;
    char *t52;
    unsigned int t54;
    unsigned int t55;
    unsigned int t56;
    char *t57;
    char *t58;
    char *t59;
    unsigned int t60;
    unsigned int t61;
    unsigned int t62;
    unsigned int t63;
    unsigned int t64;
    unsigned int t65;
    unsigned int t66;
    char *t67;
    char *t68;
    unsigned int t69;
    unsigned int t70;
    unsigned int t71;
    unsigned int t72;
    unsigned int t73;
    unsigned int t74;
    unsigned int t75;
    unsigned int t76;
    int t77;
    int t78;
    unsigned int t79;
    unsigned int t80;
    unsigned int t81;
    unsigned int t82;
    unsigned int t83;
    unsigned int t84;
    char *t86;
    unsigned int t87;
    unsigned int t88;
    unsigned int t89;
    unsigned int t90;
    unsigned int t91;
    char *t92;
    unsigned int t94;
    unsigned int t95;
    unsigned int t96;
    char *t97;
    char *t98;
    char *t99;
    unsigned int t100;
    unsigned int t101;
    unsigned int t102;
    unsigned int t103;
    unsigned int t104;
    unsigned int t105;
    unsigned int t106;
    char *t107;
    char *t108;
    unsigned int t109;
    unsigned int t110;
    unsigned int t111;
    unsigned int t112;
    unsigned int t113;
    unsigned int t114;
    unsigned int t115;
    unsigned int t116;
    int t117;
    int t118;
    unsigned int t119;
    unsigned int t120;
    unsigned int t121;
    unsigned int t122;
    unsigned int t123;
    unsigned int t124;
    char *t125;
    unsigned int t126;
    unsigned int t127;
    unsigned int t128;
    unsigned int t129;
    unsigned int t130;
    char *t131;
    char *t132;
    char *t133;
    char *t134;
    char *t136;

LAB0:    t1 = (t0 + 3416U);
    t2 = *((char **)t1);
    if (t2 == 0)
        goto LAB2;

LAB3:    goto *t2;

LAB2:    xsi_set_current_line(42, ng0);
    t2 = (t0 + 3752);
    *((int *)t2) = 1;
    t3 = (t0 + 3448);
    *((char **)t3) = t2;
    *((char **)t1) = &&LAB4;

LAB1:    return;
LAB4:    xsi_set_current_line(42, ng0);

LAB5:    xsi_set_current_line(43, ng0);
    t4 = (t0 + 1688U);
    t5 = *((char **)t4);
    memset(t6, 0, 8);
    t4 = (t5 + 4);
    t7 = *((unsigned int *)t4);
    t8 = (~(t7));
    t9 = *((unsigned int *)t5);
    t10 = (t9 & t8);
    t11 = (t10 & 1U);
    if (t11 != 0)
        goto LAB6;

LAB7:    if (*((unsigned int *)t4) != 0)
        goto LAB8;

LAB9:    t13 = (t6 + 4);
    t14 = *((unsigned int *)t6);
    t15 = *((unsigned int *)t13);
    t16 = (t14 || t15);
    if (t16 > 0)
        goto LAB10;

LAB11:    memcpy(t93, t6, 8);

LAB12:    t125 = (t93 + 4);
    t126 = *((unsigned int *)t125);
    t127 = (~(t126));
    t128 = *((unsigned int *)t93);
    t129 = (t128 & t127);
    t130 = (t129 != 0);
    if (t130 > 0)
        goto LAB44;

LAB45:    xsi_set_current_line(46, ng0);
    t2 = (t0 + 1688U);
    t3 = *((char **)t2);
    memset(t6, 0, 8);
    t2 = (t3 + 4);
    t7 = *((unsigned int *)t2);
    t8 = (~(t7));
    t9 = *((unsigned int *)t3);
    t10 = (t9 & t8);
    t11 = (t10 & 1U);
    if (t11 != 0)
        goto LAB48;

LAB49:    if (*((unsigned int *)t2) != 0)
        goto LAB50;

LAB51:    t5 = (t6 + 4);
    t14 = *((unsigned int *)t6);
    t15 = *((unsigned int *)t5);
    t16 = (t14 || t15);
    if (t16 > 0)
        goto LAB52;

LAB53:    memcpy(t41, t6, 8);

LAB54:    t39 = (t41 + 4);
    t75 = *((unsigned int *)t39);
    t76 = (~(t75));
    t79 = *((unsigned int *)t41);
    t80 = (t79 & t76);
    t81 = (t80 != 0);
    if (t81 > 0)
        goto LAB67;

LAB68:    xsi_set_current_line(49, ng0);
    t2 = (t0 + 1688U);
    t3 = *((char **)t2);
    memset(t6, 0, 8);
    t2 = (t3 + 4);
    t7 = *((unsigned int *)t2);
    t8 = (~(t7));
    t9 = *((unsigned int *)t3);
    t10 = (t9 & t8);
    t11 = (t10 & 1U);
    if (t11 != 0)
        goto LAB71;

LAB72:    if (*((unsigned int *)t2) != 0)
        goto LAB73;

LAB74:    t5 = (t6 + 4);
    t14 = *((unsigned int *)t6);
    t15 = *((unsigned int *)t5);
    t16 = (t14 || t15);
    if (t16 > 0)
        goto LAB75;

LAB76:    memcpy(t41, t6, 8);

LAB77:    t39 = (t41 + 4);
    t75 = *((unsigned int *)t39);
    t76 = (~(t75));
    t79 = *((unsigned int *)t41);
    t80 = (t79 & t76);
    t81 = (t80 != 0);
    if (t81 > 0)
        goto LAB90;

LAB91:    xsi_set_current_line(52, ng0);
    t2 = (t0 + 2248);
    t3 = (t2 + 56U);
    t4 = *((char **)t3);
    t5 = ((char*)((ng6)));
    memset(t6, 0, 8);
    t12 = (t4 + 4);
    if (*((unsigned int *)t12) != 0)
        goto LAB95;

LAB94:    t13 = (t5 + 4);
    if (*((unsigned int *)t13) != 0)
        goto LAB95;

LAB98:    if (*((unsigned int *)t4) > *((unsigned int *)t5))
        goto LAB97;

LAB96:    *((unsigned int *)t6) = 1;

LAB97:    t18 = (t6 + 4);
    t7 = *((unsigned int *)t18);
    t8 = (~(t7));
    t9 = *((unsigned int *)t6);
    t10 = (t9 & t8);
    t11 = (t10 != 0);
    if (t11 > 0)
        goto LAB99;

LAB100:    xsi_set_current_line(55, ng0);
    t2 = (t0 + 2248);
    t3 = (t2 + 56U);
    t4 = *((char **)t3);
    t5 = ((char*)((ng6)));
    memset(t6, 0, 8);
    t12 = (t4 + 4);
    if (*((unsigned int *)t12) != 0)
        goto LAB104;

LAB103:    t13 = (t5 + 4);
    if (*((unsigned int *)t13) != 0)
        goto LAB104;

LAB107:    if (*((unsigned int *)t4) > *((unsigned int *)t5))
        goto LAB105;

LAB106:    t18 = (t6 + 4);
    t7 = *((unsigned int *)t18);
    t8 = (~(t7));
    t9 = *((unsigned int *)t6);
    t10 = (t9 & t8);
    t11 = (t10 != 0);
    if (t11 > 0)
        goto LAB108;

LAB109:
LAB110:
LAB101:
LAB92:
LAB69:
LAB46:    goto LAB2;

LAB6:    *((unsigned int *)t6) = 1;
    goto LAB9;

LAB8:    t12 = (t6 + 4);
    *((unsigned int *)t6) = 1;
    *((unsigned int *)t12) = 1;
    goto LAB9;

LAB10:    t17 = (t0 + 2248);
    t18 = (t17 + 56U);
    t19 = *((char **)t18);
    t20 = ((char*)((ng5)));
    memset(t21, 0, 8);
    t22 = (t19 + 4);
    if (*((unsigned int *)t22) != 0)
        goto LAB14;

LAB13:    t23 = (t20 + 4);
    if (*((unsigned int *)t23) != 0)
        goto LAB14;

LAB17:    if (*((unsigned int *)t19) < *((unsigned int *)t20))
        goto LAB16;

LAB15:    *((unsigned int *)t21) = 1;

LAB16:    memset(t25, 0, 8);
    t26 = (t21 + 4);
    t27 = *((unsigned int *)t26);
    t28 = (~(t27));
    t29 = *((unsigned int *)t21);
    t30 = (t29 & t28);
    t31 = (t30 & 1U);
    if (t31 != 0)
        goto LAB18;

LAB19:    if (*((unsigned int *)t26) != 0)
        goto LAB20;

LAB21:    t33 = (t25 + 4);
    t34 = *((unsigned int *)t25);
    t35 = *((unsigned int *)t33);
    t36 = (t34 || t35);
    if (t36 > 0)
        goto LAB22;

LAB23:    memcpy(t53, t25, 8);

LAB24:    memset(t85, 0, 8);
    t86 = (t53 + 4);
    t87 = *((unsigned int *)t86);
    t88 = (~(t87));
    t89 = *((unsigned int *)t53);
    t90 = (t89 & t88);
    t91 = (t90 & 1U);
    if (t91 != 0)
        goto LAB37;

LAB38:    if (*((unsigned int *)t86) != 0)
        goto LAB39;

LAB40:    t94 = *((unsigned int *)t6);
    t95 = *((unsigned int *)t85);
    t96 = (t94 & t95);
    *((unsigned int *)t93) = t96;
    t97 = (t6 + 4);
    t98 = (t85 + 4);
    t99 = (t93 + 4);
    t100 = *((unsigned int *)t97);
    t101 = *((unsigned int *)t98);
    t102 = (t100 | t101);
    *((unsigned int *)t99) = t102;
    t103 = *((unsigned int *)t99);
    t104 = (t103 != 0);
    if (t104 == 1)
        goto LAB41;

LAB42:
LAB43:    goto LAB12;

LAB14:    t24 = (t21 + 4);
    *((unsigned int *)t21) = 1;
    *((unsigned int *)t24) = 1;
    goto LAB16;

LAB18:    *((unsigned int *)t25) = 1;
    goto LAB21;

LAB20:    t32 = (t25 + 4);
    *((unsigned int *)t25) = 1;
    *((unsigned int *)t32) = 1;
    goto LAB21;

LAB22:    t37 = (t0 + 2248);
    t38 = (t37 + 56U);
    t39 = *((char **)t38);
    t40 = ((char*)((ng6)));
    memset(t41, 0, 8);
    t42 = (t39 + 4);
    if (*((unsigned int *)t42) != 0)
        goto LAB26;

LAB25:    t43 = (t40 + 4);
    if (*((unsigned int *)t43) != 0)
        goto LAB26;

LAB29:    if (*((unsigned int *)t39) > *((unsigned int *)t40))
        goto LAB28;

LAB27:    *((unsigned int *)t41) = 1;

LAB28:    memset(t45, 0, 8);
    t46 = (t41 + 4);
    t47 = *((unsigned int *)t46);
    t48 = (~(t47));
    t49 = *((unsigned int *)t41);
    t50 = (t49 & t48);
    t51 = (t50 & 1U);
    if (t51 != 0)
        goto LAB30;

LAB31:    if (*((unsigned int *)t46) != 0)
        goto LAB32;

LAB33:    t54 = *((unsigned int *)t25);
    t55 = *((unsigned int *)t45);
    t56 = (t54 & t55);
    *((unsigned int *)t53) = t56;
    t57 = (t25 + 4);
    t58 = (t45 + 4);
    t59 = (t53 + 4);
    t60 = *((unsigned int *)t57);
    t61 = *((unsigned int *)t58);
    t62 = (t60 | t61);
    *((unsigned int *)t59) = t62;
    t63 = *((unsigned int *)t59);
    t64 = (t63 != 0);
    if (t64 == 1)
        goto LAB34;

LAB35:
LAB36:    goto LAB24;

LAB26:    t44 = (t41 + 4);
    *((unsigned int *)t41) = 1;
    *((unsigned int *)t44) = 1;
    goto LAB28;

LAB30:    *((unsigned int *)t45) = 1;
    goto LAB33;

LAB32:    t52 = (t45 + 4);
    *((unsigned int *)t45) = 1;
    *((unsigned int *)t52) = 1;
    goto LAB33;

LAB34:    t65 = *((unsigned int *)t53);
    t66 = *((unsigned int *)t59);
    *((unsigned int *)t53) = (t65 | t66);
    t67 = (t25 + 4);
    t68 = (t45 + 4);
    t69 = *((unsigned int *)t25);
    t70 = (~(t69));
    t71 = *((unsigned int *)t67);
    t72 = (~(t71));
    t73 = *((unsigned int *)t45);
    t74 = (~(t73));
    t75 = *((unsigned int *)t68);
    t76 = (~(t75));
    t77 = (t70 & t72);
    t78 = (t74 & t76);
    t79 = (~(t77));
    t80 = (~(t78));
    t81 = *((unsigned int *)t59);
    *((unsigned int *)t59) = (t81 & t79);
    t82 = *((unsigned int *)t59);
    *((unsigned int *)t59) = (t82 & t80);
    t83 = *((unsigned int *)t53);
    *((unsigned int *)t53) = (t83 & t79);
    t84 = *((unsigned int *)t53);
    *((unsigned int *)t53) = (t84 & t80);
    goto LAB36;

LAB37:    *((unsigned int *)t85) = 1;
    goto LAB40;

LAB39:    t92 = (t85 + 4);
    *((unsigned int *)t85) = 1;
    *((unsigned int *)t92) = 1;
    goto LAB40;

LAB41:    t105 = *((unsigned int *)t93);
    t106 = *((unsigned int *)t99);
    *((unsigned int *)t93) = (t105 | t106);
    t107 = (t6 + 4);
    t108 = (t85 + 4);
    t109 = *((unsigned int *)t6);
    t110 = (~(t109));
    t111 = *((unsigned int *)t107);
    t112 = (~(t111));
    t113 = *((unsigned int *)t85);
    t114 = (~(t113));
    t115 = *((unsigned int *)t108);
    t116 = (~(t115));
    t117 = (t110 & t112);
    t118 = (t114 & t116);
    t119 = (~(t117));
    t120 = (~(t118));
    t121 = *((unsigned int *)t99);
    *((unsigned int *)t99) = (t121 & t119);
    t122 = *((unsigned int *)t99);
    *((unsigned int *)t99) = (t122 & t120);
    t123 = *((unsigned int *)t93);
    *((unsigned int *)t93) = (t123 & t119);
    t124 = *((unsigned int *)t93);
    *((unsigned int *)t93) = (t124 & t120);
    goto LAB43;

LAB44:    xsi_set_current_line(43, ng0);

LAB47:    xsi_set_current_line(44, ng0);
    t131 = (t0 + 2248);
    t132 = (t131 + 56U);
    t133 = *((char **)t132);
    t134 = ((char*)((ng5)));
    memset(t135, 0, 8);
    xsi_vlog_unsigned_minus(t135, 11, t133, 11, t134, 11);
    t136 = (t0 + 2088);
    xsi_vlogvar_assign_value(t136, t135, 0, 0, 11);
    goto LAB46;

LAB48:    *((unsigned int *)t6) = 1;
    goto LAB51;

LAB50:    t4 = (t6 + 4);
    *((unsigned int *)t6) = 1;
    *((unsigned int *)t4) = 1;
    goto LAB51;

LAB52:    t12 = (t0 + 2248);
    t13 = (t12 + 56U);
    t17 = *((char **)t13);
    t18 = ((char*)((ng5)));
    memset(t21, 0, 8);
    t19 = (t17 + 4);
    if (*((unsigned int *)t19) != 0)
        goto LAB56;

LAB55:    t20 = (t18 + 4);
    if (*((unsigned int *)t20) != 0)
        goto LAB56;

LAB59:    if (*((unsigned int *)t17) < *((unsigned int *)t18))
        goto LAB57;

LAB58:    memset(t25, 0, 8);
    t23 = (t21 + 4);
    t27 = *((unsigned int *)t23);
    t28 = (~(t27));
    t29 = *((unsigned int *)t21);
    t30 = (t29 & t28);
    t31 = (t30 & 1U);
    if (t31 != 0)
        goto LAB60;

LAB61:    if (*((unsigned int *)t23) != 0)
        goto LAB62;

LAB63:    t34 = *((unsigned int *)t6);
    t35 = *((unsigned int *)t25);
    t36 = (t34 & t35);
    *((unsigned int *)t41) = t36;
    t26 = (t6 + 4);
    t32 = (t25 + 4);
    t33 = (t41 + 4);
    t47 = *((unsigned int *)t26);
    t48 = *((unsigned int *)t32);
    t49 = (t47 | t48);
    *((unsigned int *)t33) = t49;
    t50 = *((unsigned int *)t33);
    t51 = (t50 != 0);
    if (t51 == 1)
        goto LAB64;

LAB65:
LAB66:    goto LAB54;

LAB56:    t22 = (t21 + 4);
    *((unsigned int *)t21) = 1;
    *((unsigned int *)t22) = 1;
    goto LAB58;

LAB57:    *((unsigned int *)t21) = 1;
    goto LAB58;

LAB60:    *((unsigned int *)t25) = 1;
    goto LAB63;

LAB62:    t24 = (t25 + 4);
    *((unsigned int *)t25) = 1;
    *((unsigned int *)t24) = 1;
    goto LAB63;

LAB64:    t54 = *((unsigned int *)t41);
    t55 = *((unsigned int *)t33);
    *((unsigned int *)t41) = (t54 | t55);
    t37 = (t6 + 4);
    t38 = (t25 + 4);
    t56 = *((unsigned int *)t6);
    t60 = (~(t56));
    t61 = *((unsigned int *)t37);
    t62 = (~(t61));
    t63 = *((unsigned int *)t25);
    t64 = (~(t63));
    t65 = *((unsigned int *)t38);
    t66 = (~(t65));
    t77 = (t60 & t62);
    t78 = (t64 & t66);
    t69 = (~(t77));
    t70 = (~(t78));
    t71 = *((unsigned int *)t33);
    *((unsigned int *)t33) = (t71 & t69);
    t72 = *((unsigned int *)t33);
    *((unsigned int *)t33) = (t72 & t70);
    t73 = *((unsigned int *)t41);
    *((unsigned int *)t41) = (t73 & t69);
    t74 = *((unsigned int *)t41);
    *((unsigned int *)t41) = (t74 & t70);
    goto LAB66;

LAB67:    xsi_set_current_line(46, ng0);

LAB70:    xsi_set_current_line(47, ng0);
    t40 = ((char*)((ng7)));
    t42 = (t0 + 2088);
    xsi_vlogvar_assign_value(t42, t40, 0, 0, 11);
    goto LAB69;

LAB71:    *((unsigned int *)t6) = 1;
    goto LAB74;

LAB73:    t4 = (t6 + 4);
    *((unsigned int *)t6) = 1;
    *((unsigned int *)t4) = 1;
    goto LAB74;

LAB75:    t12 = (t0 + 2248);
    t13 = (t12 + 56U);
    t17 = *((char **)t13);
    t18 = ((char*)((ng6)));
    memset(t21, 0, 8);
    t19 = (t17 + 4);
    if (*((unsigned int *)t19) != 0)
        goto LAB79;

LAB78:    t20 = (t18 + 4);
    if (*((unsigned int *)t20) != 0)
        goto LAB79;

LAB82:    if (*((unsigned int *)t17) > *((unsigned int *)t18))
        goto LAB80;

LAB81:    memset(t25, 0, 8);
    t23 = (t21 + 4);
    t27 = *((unsigned int *)t23);
    t28 = (~(t27));
    t29 = *((unsigned int *)t21);
    t30 = (t29 & t28);
    t31 = (t30 & 1U);
    if (t31 != 0)
        goto LAB83;

LAB84:    if (*((unsigned int *)t23) != 0)
        goto LAB85;

LAB86:    t34 = *((unsigned int *)t6);
    t35 = *((unsigned int *)t25);
    t36 = (t34 & t35);
    *((unsigned int *)t41) = t36;
    t26 = (t6 + 4);
    t32 = (t25 + 4);
    t33 = (t41 + 4);
    t47 = *((unsigned int *)t26);
    t48 = *((unsigned int *)t32);
    t49 = (t47 | t48);
    *((unsigned int *)t33) = t49;
    t50 = *((unsigned int *)t33);
    t51 = (t50 != 0);
    if (t51 == 1)
        goto LAB87;

LAB88:
LAB89:    goto LAB77;

LAB79:    t22 = (t21 + 4);
    *((unsigned int *)t21) = 1;
    *((unsigned int *)t22) = 1;
    goto LAB81;

LAB80:    *((unsigned int *)t21) = 1;
    goto LAB81;

LAB83:    *((unsigned int *)t25) = 1;
    goto LAB86;

LAB85:    t24 = (t25 + 4);
    *((unsigned int *)t25) = 1;
    *((unsigned int *)t24) = 1;
    goto LAB86;

LAB87:    t54 = *((unsigned int *)t41);
    t55 = *((unsigned int *)t33);
    *((unsigned int *)t41) = (t54 | t55);
    t37 = (t6 + 4);
    t38 = (t25 + 4);
    t56 = *((unsigned int *)t6);
    t60 = (~(t56));
    t61 = *((unsigned int *)t37);
    t62 = (~(t61));
    t63 = *((unsigned int *)t25);
    t64 = (~(t63));
    t65 = *((unsigned int *)t38);
    t66 = (~(t65));
    t77 = (t60 & t62);
    t78 = (t64 & t66);
    t69 = (~(t77));
    t70 = (~(t78));
    t71 = *((unsigned int *)t33);
    *((unsigned int *)t33) = (t71 & t69);
    t72 = *((unsigned int *)t33);
    *((unsigned int *)t33) = (t72 & t70);
    t73 = *((unsigned int *)t41);
    *((unsigned int *)t41) = (t73 & t69);
    t74 = *((unsigned int *)t41);
    *((unsigned int *)t41) = (t74 & t70);
    goto LAB89;

LAB90:    xsi_set_current_line(49, ng0);

LAB93:    xsi_set_current_line(50, ng0);
    t40 = ((char*)((ng8)));
    t42 = ((char*)((ng5)));
    memset(t45, 0, 8);
    xsi_vlog_unsigned_minus(t45, 11, t40, 11, t42, 11);
    t43 = (t0 + 2248);
    t44 = (t43 + 56U);
    t46 = *((char **)t44);
    t52 = ((char*)((ng8)));
    memset(t53, 0, 8);
    xsi_vlog_unsigned_minus(t53, 11, t46, 11, t52, 11);
    memset(t85, 0, 8);
    xsi_vlog_unsigned_add(t85, 11, t45, 11, t53, 11);
    t57 = (t0 + 2088);
    xsi_vlogvar_assign_value(t57, t85, 0, 0, 11);
    goto LAB92;

LAB95:    t17 = (t6 + 4);
    *((unsigned int *)t6) = 1;
    *((unsigned int *)t17) = 1;
    goto LAB97;

LAB99:    xsi_set_current_line(52, ng0);

LAB102:    xsi_set_current_line(53, ng0);
    t19 = (t0 + 2248);
    t20 = (t19 + 56U);
    t22 = *((char **)t20);
    t23 = (t0 + 2088);
    xsi_vlogvar_assign_value(t23, t22, 0, 0, 11);
    goto LAB101;

LAB104:    t17 = (t6 + 4);
    *((unsigned int *)t6) = 1;
    *((unsigned int *)t17) = 1;
    goto LAB106;

LAB105:    *((unsigned int *)t6) = 1;
    goto LAB106;

LAB108:    xsi_set_current_line(55, ng0);

LAB111:    xsi_set_current_line(56, ng0);
    t19 = ((char*)((ng6)));
    t20 = (t0 + 2088);
    xsi_vlogvar_assign_value(t20, t19, 0, 0, 11);
    goto LAB110;

}


extern void work_m_00000000000279323812_0491843782_init()
{
	static char *pe[] = {(void *)Always_33_0,(void *)Always_42_1};
	xsi_register_didat("work_m_00000000000279323812_0491843782", "isim/testando_filtragempeso_isim_beh.exe.sim/work/m_00000000000279323812_0491843782.didat");
	xsi_register_executes(pe);
}
