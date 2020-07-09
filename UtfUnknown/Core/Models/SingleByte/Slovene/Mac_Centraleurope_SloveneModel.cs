﻿﻿/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is Mozilla Communicator client code.
 *
 * The Initial Developer of the Original Code is
 * Netscape Communications Corporation.
 * Portions created by the Initial Developer are Copyright (C) 1998
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

/*
* The following part was imported from https://gitlab.freedesktop.org/uchardet/uchardet
* The implementation of this feature was originally done on https://gitlab.freedesktop.org/uchardet/uchardet/blob/master/src/LangModels/LangSloveneModel.cpp
* and adjusted to language specific support.
*/

namespace UtfUnknown.Core.Models.SingleByte.Slovene
{
    public class Mac_Centraleurope_SloveneModel : SloveneModel
    {
        // Generated by BuildLangModel.py
        // On: 2016-09-28 22:06:46.134717

        // Character Mapping Table:
        // ILL: illegal character.
        // CTR: control character specific to the charset.
        // RET: carriage/return.
        // SYM: symbol (punctuation) that does not belong to word.
        // NUM: 0 - 9.

        // Other characters are ordered by probabilities
        // (0 is the most common character in the language).

        // Orders are generic to a language. So the codepoint with order X in
        // CHARSET1 maps to the same character as the codepoint with the same
        // order X in CHARSET2 for the same language.
        // As such, it is possible to get missing order. For instance the
        // ligature of 'o' and 'e' exists in ISO-8859-15 but not in ISO-8859-1
        // even though they are both used for French. Same for the euro sign.

        private static byte[] CHAR_TO_ORDER_MAP = {
          CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,RET,CTR,CTR,RET,CTR,CTR, /* 0X */
          CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR,CTR, /* 1X */
          SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM, /* 2X */
          NUM,NUM,NUM,NUM,NUM,NUM,NUM,NUM,NUM,NUM,SYM,SYM,SYM,SYM,SYM,SYM, /* 3X */
          SYM,  0, 18, 19, 13,  1, 24, 17, 20,  2,  8, 12,  9, 14,  4,  3, /* 4X */
           11, 28,  5,  6,  7, 16, 10, 27, 25, 26, 15,SYM,SYM,SYM,SYM,SYM, /* 5X */
          SYM,  0, 18, 19, 13,  1, 24, 17, 20,  2,  8, 12,  9, 14,  4,  3, /* 6X */
           11, 28,  5,  6,  7, 16, 10, 27, 25, 26, 15,SYM,SYM,SYM,SYM,CTR, /* 7X */
          208,209,210, 29,211,212,213, 32,214, 21,215, 21, 37, 37, 29,216, /* 8X */
          217,218, 30,219, 38, 38,220, 31,221, 35,222,223, 39,224,225,226, /* 9X */
          SYM,SYM,227,SYM,SYM,SYM,SYM,228,SYM,SYM,SYM,229,SYM,SYM,230,231, /* AX */
          232,233,SYM,SYM,234,235,SYM,SYM,236,237,238,239,240,241,242,243, /* BX */
          244,245,SYM,SYM,246,247,SYM,SYM,SYM,SYM,SYM,248,249,249,249,249, /* CX */
          SYM,SYM,SYM,SYM,SYM,SYM,SYM,SYM,249,249,249,249,SYM,SYM,249,249, /* DX */
          249, 22,SYM,SYM, 22,249,249, 32,249,249, 30, 23, 23,249, 31, 35, /* EX */
          249,249, 39,249,249,249,249,249, 40, 40,249,249,249,249,249,SYM, /* FX */
        };
        /*X0  X1  X2  X3  X4  X5  X6  X7  X8  X9  XA  XB  XC  XD  XE  XF */

        public Mac_Centraleurope_SloveneModel() : base(CHAR_TO_ORDER_MAP, CodepageName.X_MAC_CE)
        {
        }
    }
}
