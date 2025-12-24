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
static const char *ng0 = "C:/Users/joaoc/Desktop/PROJETOS/Projeto_F1v2/bin2bcd_11_16.v";
static int ng1[] = {0, 0};
static int ng2[] = {11, 0};
static int ng3[] = {5, 0};
static int ng4[] = {3, 0};
static int ng5[] = {7, 0};
static int ng6[] = {4, 0};
static int ng7[] = {8, 0};
static int ng8[] = {15, 0};
static int ng9[] = {12, 0};
static unsigned int ng10[] = {0U, 0U};
static int ng11[] = {10, 0};
static int ng12[] = {1, 0};



static void Always_29_0(char *t0)
{
    char t6[8];
    char t13[8];
    char t26[8];
    char t36[8];
    char t49[8];
    char t51[8];
    char t52[8];
    char t53[8];
    char *t1;
    char *t2;
    char *t3;
    char *t4;
    char *t5;
    char *t7;
    unsigned int t8;
    unsigned int t9;
    unsigned int t10;
    unsigned int t11;
    unsigned int t12;
    char *t14;
    char *t15;
    char *t16;
    char *t17;
    char *t18;
    unsigned int t19;
    unsigned int t20;
    unsigned int t21;
    unsigned int t22;
    unsigned int t23;
    unsigned int t24;
    char *t25;
    char *t27;
    char *t28;
    char *t29;
    char *t30;
    unsigned int t31;
    unsigned int t32;
    unsigned int t33;
    unsigned int t34;
    unsigned int t35;
    char *t37;
    char *t38;
    char *t39;
    char *t40;
    char *t41;
    unsigned int t42;
    unsigned int t43;
    unsigned int t44;
    unsigned int t45;
    unsigned int t46;
    unsigned int t47;
    char *t48;
    char *t50;
    char *t54;
    char *t55;
    char *t56;
    char *t57;
    char *t58;
    char *t59;
    unsigned int t60;
    int t61;
    char *t62;
    unsigned int t63;
    int t64;
    int t65;
    char *t66;
    unsigned int t67;
    int t68;
    int t69;
    unsigned int t70;
    int t71;
    unsigned int t72;
    unsigned int t73;
    int t74;
    int t75;

LAB0:    t1 = (t0 + 2528U);
    t2 = *((char **)t1);
    if (t2 == 0)
        goto LAB2;

LAB3:    goto *t2;

LAB2:    xsi_set_current_line(29, ng0);
    t2 = (t0 + 2848);
    *((int *)t2) = 1;
    t3 = (t0 + 2560);
    *((char **)t3) = t2;
    *((char **)t1) = &&LAB4;

LAB1:    return;
LAB4:    xsi_set_current_line(29, ng0);

LAB5:    xsi_set_current_line(30, ng0);
    t4 = ((char*)((ng1)));
    t5 = (t0 + 1448);
    xsi_vlogvar_assign_value(t5, t4, 0, 0, 16);
    xsi_set_current_line(32, ng0);
    xsi_set_current_line(32, ng0);
    t2 = ((char*)((ng1)));
    t3 = (t0 + 1608);
    xsi_vlogvar_assign_value(t3, t2, 0, 0, 32);

LAB6:    t2 = (t0 + 1608);
    t3 = (t2 + 56U);
    t4 = *((char **)t3);
    t5 = ((char*)((ng2)));
    memset(t6, 0, 8);
    xsi_vlog_signed_less(t6, 32, t4, 32, t5, 32);
    t7 = (t6 + 4);
    t8 = *((unsigned int *)t7);
    t9 = (~(t8));
    t10 = *((unsigned int *)t6);
    t11 = (t10 & t9);
    t12 = (t11 != 0);
    if (t12 > 0)
        goto LAB7;

LAB8:    goto LAB2;

LAB7:    xsi_set_current_line(32, ng0);

LAB9:    xsi_set_current_line(34, ng0);
    t14 = (t0 + 1448);
    t15 = (t14 + 56U);
    t16 = *((char **)t15);
    memset(t13, 0, 8);
    t17 = (t13 + 4);
    t18 = (t16 + 4);
    t19 = *((unsigned int *)t16);
    t20 = (t19 >> 0);
    *((unsigned int *)t13) = t20;
    t21 = *((unsigned int *)t18);
    t22 = (t21 >> 0);
    *((unsigned int *)t17) = t22;
    t23 = *((unsigned int *)t13);
    *((unsigned int *)t13) = (t23 & 15U);
    t24 = *((unsigned int *)t17);
    *((unsigned int *)t17) = (t24 & 15U);
    t25 = ((char*)((ng3)));
    memset(t26, 0, 8);
    t27 = (t13 + 4);
    if (*((unsigned int *)t27) != 0)
        goto LAB11;

LAB10:    t28 = (t25 + 4);
    if (*((unsigned int *)t28) != 0)
        goto LAB11;

LAB14:    if (*((unsigned int *)t13) < *((unsigned int *)t25))
        goto LAB13;

LAB12:    *((unsigned int *)t26) = 1;

LAB13:    t30 = (t26 + 4);
    t31 = *((unsigned int *)t30);
    t32 = (~(t31));
    t33 = *((unsigned int *)t26);
    t34 = (t33 & t32);
    t35 = (t34 != 0);
    if (t35 > 0)
        goto LAB15;

LAB16:
LAB17:    xsi_set_current_line(35, ng0);
    t2 = (t0 + 1448);
    t3 = (t2 + 56U);
    t4 = *((char **)t3);
    memset(t6, 0, 8);
    t5 = (t6 + 4);
    t7 = (t4 + 4);
    t8 = *((unsigned int *)t4);
    t9 = (t8 >> 4);
    *((unsigned int *)t6) = t9;
    t10 = *((unsigned int *)t7);
    t11 = (t10 >> 4);
    *((unsigned int *)t5) = t11;
    t12 = *((unsigned int *)t6);
    *((unsigned int *)t6) = (t12 & 15U);
    t19 = *((unsigned int *)t5);
    *((unsigned int *)t5) = (t19 & 15U);
    t14 = ((char*)((ng3)));
    memset(t13, 0, 8);
    t15 = (t6 + 4);
    if (*((unsigned int *)t15) != 0)
        goto LAB21;

LAB20:    t16 = (t14 + 4);
    if (*((unsigned int *)t16) != 0)
        goto LAB21;

LAB24:    if (*((unsigned int *)t6) < *((unsigned int *)t14))
        goto LAB23;

LAB22:    *((unsigned int *)t13) = 1;

LAB23:    t18 = (t13 + 4);
    t20 = *((unsigned int *)t18);
    t21 = (~(t20));
    t22 = *((unsigned int *)t13);
    t23 = (t22 & t21);
    t24 = (t23 != 0);
    if (t24 > 0)
        goto LAB25;

LAB26:
LAB27:    xsi_set_current_line(36, ng0);
    t2 = (t0 + 1448);
    t3 = (t2 + 56U);
    t4 = *((char **)t3);
    memset(t6, 0, 8);
    t5 = (t6 + 4);
    t7 = (t4 + 4);
    t8 = *((unsigned int *)t4);
    t9 = (t8 >> 8);
    *((unsigned int *)t6) = t9;
    t10 = *((unsigned int *)t7);
    t11 = (t10 >> 8);
    *((unsigned int *)t5) = t11;
    t12 = *((unsigned int *)t6);
    *((unsigned int *)t6) = (t12 & 15U);
    t19 = *((unsigned int *)t5);
    *((unsigned int *)t5) = (t19 & 15U);
    t14 = ((char*)((ng3)));
    memset(t13, 0, 8);
    t15 = (t6 + 4);
    if (*((unsigned int *)t15) != 0)
        goto LAB31;

LAB30:    t16 = (t14 + 4);
    if (*((unsigned int *)t16) != 0)
        goto LAB31;

LAB34:    if (*((unsigned int *)t6) < *((unsigned int *)t14))
        goto LAB33;

LAB32:    *((unsigned int *)t13) = 1;

LAB33:    t18 = (t13 + 4);
    t20 = *((unsigned int *)t18);
    t21 = (~(t20));
    t22 = *((unsigned int *)t13);
    t23 = (t22 & t21);
    t24 = (t23 != 0);
    if (t24 > 0)
        goto LAB35;

LAB36:
LAB37:    xsi_set_current_line(37, ng0);
    t2 = (t0 + 1448);
    t3 = (t2 + 56U);
    t4 = *((char **)t3);
    memset(t6, 0, 8);
    t5 = (t6 + 4);
    t7 = (t4 + 4);
    t8 = *((unsigned int *)t4);
    t9 = (t8 >> 12);
    *((unsigned int *)t6) = t9;
    t10 = *((unsigned int *)t7);
    t11 = (t10 >> 12);
    *((unsigned int *)t5) = t11;
    t12 = *((unsigned int *)t6);
    *((unsigned int *)t6) = (t12 & 15U);
    t19 = *((unsigned int *)t5);
    *((unsigned int *)t5) = (t19 & 15U);
    t14 = ((char*)((ng3)));
    memset(t13, 0, 8);
    t15 = (t6 + 4);
    if (*((unsigned int *)t15) != 0)
        goto LAB41;

LAB40:    t16 = (t14 + 4);
    if (*((unsigned int *)t16) != 0)
        goto LAB41;

LAB44:    if (*((unsigned int *)t6) < *((unsigned int *)t14))
        goto LAB43;

LAB42:    *((unsigned int *)t13) = 1;

LAB43:    t18 = (t13 + 4);
    t20 = *((unsigned int *)t18);
    t21 = (~(t20));
    t22 = *((unsigned int *)t13);
    t23 = (t22 & t21);
    t24 = (t23 != 0);
    if (t24 > 0)
        goto LAB45;

LAB46:
LAB47:    xsi_set_current_line(40, ng0);
    t2 = ((char*)((ng10)));
    t3 = (t0 + 1448);
    t4 = (t3 + 56U);
    t5 = *((char **)t4);
    memset(t13, 0, 8);
    t7 = (t13 + 4);
    t14 = (t5 + 4);
    t8 = *((unsigned int *)t5);
    t9 = (t8 >> 0);
    *((unsigned int *)t13) = t9;
    t10 = *((unsigned int *)t14);
    t11 = (t10 >> 0);
    *((unsigned int *)t7) = t11;
    t12 = *((unsigned int *)t13);
    *((unsigned int *)t13) = (t12 & 32767U);
    t19 = *((unsigned int *)t7);
    *((unsigned int *)t7) = (t19 & 32767U);
    xsi_vlogtype_concat(t6, 16, 16, 2U, t13, 15, t2, 1);
    t15 = (t0 + 1448);
    xsi_vlogvar_assign_value(t15, t6, 0, 0, 16);
    xsi_set_current_line(41, ng0);
    t2 = (t0 + 1048U);
    t3 = *((char **)t2);
    t2 = (t0 + 1008U);
    t4 = (t2 + 72U);
    t5 = *((char **)t4);
    t7 = ((char*)((ng11)));
    t14 = (t0 + 1608);
    t15 = (t14 + 56U);
    t16 = *((char **)t15);
    memset(t13, 0, 8);
    xsi_vlog_signed_minus(t13, 32, t7, 32, t16, 32);
    xsi_vlog_generic_get_index_select_value(t6, 1, t3, t5, 2, t13, 32, 1);
    t17 = (t0 + 1448);
    t18 = (t0 + 1448);
    t25 = (t18 + 72U);
    t27 = *((char **)t25);
    t28 = ((char*)((ng1)));
    xsi_vlog_generic_convert_bit_index(t26, t27, 2, t28, 32, 1);
    t29 = (t26 + 4);
    t8 = *((unsigned int *)t29);
    t61 = (!(t8));
    if (t61 == 1)
        goto LAB50;

LAB51:    xsi_set_current_line(32, ng0);
    t2 = (t0 + 1608);
    t3 = (t2 + 56U);
    t4 = *((char **)t3);
    t5 = ((char*)((ng12)));
    memset(t6, 0, 8);
    xsi_vlog_signed_add(t6, 32, t4, 32, t5, 32);
    t7 = (t0 + 1608);
    xsi_vlogvar_assign_value(t7, t6, 0, 0, 32);
    goto LAB6;

LAB11:    t29 = (t26 + 4);
    *((unsigned int *)t26) = 1;
    *((unsigned int *)t29) = 1;
    goto LAB13;

LAB15:    xsi_set_current_line(34, ng0);
    t37 = (t0 + 1448);
    t38 = (t37 + 56U);
    t39 = *((char **)t38);
    memset(t36, 0, 8);
    t40 = (t36 + 4);
    t41 = (t39 + 4);
    t42 = *((unsigned int *)t39);
    t43 = (t42 >> 0);
    *((unsigned int *)t36) = t43;
    t44 = *((unsigned int *)t41);
    t45 = (t44 >> 0);
    *((unsigned int *)t40) = t45;
    t46 = *((unsigned int *)t36);
    *((unsigned int *)t36) = (t46 & 15U);
    t47 = *((unsigned int *)t40);
    *((unsigned int *)t40) = (t47 & 15U);
    t48 = ((char*)((ng4)));
    memset(t49, 0, 8);
    xsi_vlog_unsigned_add(t49, 32, t36, 32, t48, 32);
    t50 = (t0 + 1448);
    t54 = (t0 + 1448);
    t55 = (t54 + 72U);
    t56 = *((char **)t55);
    t57 = ((char*)((ng4)));
    t58 = ((char*)((ng1)));
    xsi_vlog_convert_partindices(t51, t52, t53, ((int*)(t56)), 2, t57, 32, 1, t58, 32, 1);
    t59 = (t51 + 4);
    t60 = *((unsigned int *)t59);
    t61 = (!(t60));
    t62 = (t52 + 4);
    t63 = *((unsigned int *)t62);
    t64 = (!(t63));
    t65 = (t61 && t64);
    t66 = (t53 + 4);
    t67 = *((unsigned int *)t66);
    t68 = (!(t67));
    t69 = (t65 && t68);
    if (t69 == 1)
        goto LAB18;

LAB19:    goto LAB17;

LAB18:    t70 = *((unsigned int *)t53);
    t71 = (t70 + 0);
    t72 = *((unsigned int *)t51);
    t73 = *((unsigned int *)t52);
    t74 = (t72 - t73);
    t75 = (t74 + 1);
    xsi_vlogvar_assign_value(t50, t49, t71, *((unsigned int *)t52), t75);
    goto LAB19;

LAB21:    t17 = (t13 + 4);
    *((unsigned int *)t13) = 1;
    *((unsigned int *)t17) = 1;
    goto LAB23;

LAB25:    xsi_set_current_line(35, ng0);
    t25 = (t0 + 1448);
    t27 = (t25 + 56U);
    t28 = *((char **)t27);
    memset(t26, 0, 8);
    t29 = (t26 + 4);
    t30 = (t28 + 4);
    t31 = *((unsigned int *)t28);
    t32 = (t31 >> 4);
    *((unsigned int *)t26) = t32;
    t33 = *((unsigned int *)t30);
    t34 = (t33 >> 4);
    *((unsigned int *)t29) = t34;
    t35 = *((unsigned int *)t26);
    *((unsigned int *)t26) = (t35 & 15U);
    t42 = *((unsigned int *)t29);
    *((unsigned int *)t29) = (t42 & 15U);
    t37 = ((char*)((ng4)));
    memset(t36, 0, 8);
    xsi_vlog_unsigned_add(t36, 32, t26, 32, t37, 32);
    t38 = (t0 + 1448);
    t39 = (t0 + 1448);
    t40 = (t39 + 72U);
    t41 = *((char **)t40);
    t48 = ((char*)((ng5)));
    t50 = ((char*)((ng6)));
    xsi_vlog_convert_partindices(t49, t51, t52, ((int*)(t41)), 2, t48, 32, 1, t50, 32, 1);
    t54 = (t49 + 4);
    t43 = *((unsigned int *)t54);
    t61 = (!(t43));
    t55 = (t51 + 4);
    t44 = *((unsigned int *)t55);
    t64 = (!(t44));
    t65 = (t61 && t64);
    t56 = (t52 + 4);
    t45 = *((unsigned int *)t56);
    t68 = (!(t45));
    t69 = (t65 && t68);
    if (t69 == 1)
        goto LAB28;

LAB29:    goto LAB27;

LAB28:    t46 = *((unsigned int *)t52);
    t71 = (t46 + 0);
    t47 = *((unsigned int *)t49);
    t60 = *((unsigned int *)t51);
    t74 = (t47 - t60);
    t75 = (t74 + 1);
    xsi_vlogvar_assign_value(t38, t36, t71, *((unsigned int *)t51), t75);
    goto LAB29;

LAB31:    t17 = (t13 + 4);
    *((unsigned int *)t13) = 1;
    *((unsigned int *)t17) = 1;
    goto LAB33;

LAB35:    xsi_set_current_line(36, ng0);
    t25 = (t0 + 1448);
    t27 = (t25 + 56U);
    t28 = *((char **)t27);
    memset(t26, 0, 8);
    t29 = (t26 + 4);
    t30 = (t28 + 4);
    t31 = *((unsigned int *)t28);
    t32 = (t31 >> 8);
    *((unsigned int *)t26) = t32;
    t33 = *((unsigned int *)t30);
    t34 = (t33 >> 8);
    *((unsigned int *)t29) = t34;
    t35 = *((unsigned int *)t26);
    *((unsigned int *)t26) = (t35 & 15U);
    t42 = *((unsigned int *)t29);
    *((unsigned int *)t29) = (t42 & 15U);
    t37 = ((char*)((ng4)));
    memset(t36, 0, 8);
    xsi_vlog_unsigned_add(t36, 32, t26, 32, t37, 32);
    t38 = (t0 + 1448);
    t39 = (t0 + 1448);
    t40 = (t39 + 72U);
    t41 = *((char **)t40);
    t48 = ((char*)((ng2)));
    t50 = ((char*)((ng7)));
    xsi_vlog_convert_partindices(t49, t51, t52, ((int*)(t41)), 2, t48, 32, 1, t50, 32, 1);
    t54 = (t49 + 4);
    t43 = *((unsigned int *)t54);
    t61 = (!(t43));
    t55 = (t51 + 4);
    t44 = *((unsigned int *)t55);
    t64 = (!(t44));
    t65 = (t61 && t64);
    t56 = (t52 + 4);
    t45 = *((unsigned int *)t56);
    t68 = (!(t45));
    t69 = (t65 && t68);
    if (t69 == 1)
        goto LAB38;

LAB39:    goto LAB37;

LAB38:    t46 = *((unsigned int *)t52);
    t71 = (t46 + 0);
    t47 = *((unsigned int *)t49);
    t60 = *((unsigned int *)t51);
    t74 = (t47 - t60);
    t75 = (t74 + 1);
    xsi_vlogvar_assign_value(t38, t36, t71, *((unsigned int *)t51), t75);
    goto LAB39;

LAB41:    t17 = (t13 + 4);
    *((unsigned int *)t13) = 1;
    *((unsigned int *)t17) = 1;
    goto LAB43;

LAB45:    xsi_set_current_line(37, ng0);
    t25 = (t0 + 1448);
    t27 = (t25 + 56U);
    t28 = *((char **)t27);
    memset(t26, 0, 8);
    t29 = (t26 + 4);
    t30 = (t28 + 4);
    t31 = *((unsigned int *)t28);
    t32 = (t31 >> 12);
    *((unsigned int *)t26) = t32;
    t33 = *((unsigned int *)t30);
    t34 = (t33 >> 12);
    *((unsigned int *)t29) = t34;
    t35 = *((unsigned int *)t26);
    *((unsigned int *)t26) = (t35 & 15U);
    t42 = *((unsigned int *)t29);
    *((unsigned int *)t29) = (t42 & 15U);
    t37 = ((char*)((ng4)));
    memset(t36, 0, 8);
    xsi_vlog_unsigned_add(t36, 32, t26, 32, t37, 32);
    t38 = (t0 + 1448);
    t39 = (t0 + 1448);
    t40 = (t39 + 72U);
    t41 = *((char **)t40);
    t48 = ((char*)((ng8)));
    t50 = ((char*)((ng9)));
    xsi_vlog_convert_partindices(t49, t51, t52, ((int*)(t41)), 2, t48, 32, 1, t50, 32, 1);
    t54 = (t49 + 4);
    t43 = *((unsigned int *)t54);
    t61 = (!(t43));
    t55 = (t51 + 4);
    t44 = *((unsigned int *)t55);
    t64 = (!(t44));
    t65 = (t61 && t64);
    t56 = (t52 + 4);
    t45 = *((unsigned int *)t56);
    t68 = (!(t45));
    t69 = (t65 && t68);
    if (t69 == 1)
        goto LAB48;

LAB49:    goto LAB47;

LAB48:    t46 = *((unsigned int *)t52);
    t71 = (t46 + 0);
    t47 = *((unsigned int *)t49);
    t60 = *((unsigned int *)t51);
    t74 = (t47 - t60);
    t75 = (t74 + 1);
    xsi_vlogvar_assign_value(t38, t36, t71, *((unsigned int *)t51), t75);
    goto LAB49;

LAB50:    xsi_vlogvar_assign_value(t17, t6, 0, *((unsigned int *)t26), 1);
    goto LAB51;

}


extern void work_m_00000000001437748857_3873283891_init()
{
	static char *pe[] = {(void *)Always_29_0};
	xsi_register_didat("work_m_00000000001437748857_3873283891", "isim/valores_esquema_isim_beh.exe.sim/work/m_00000000001437748857_3873283891.didat");
	xsi_register_executes(pe);
}
