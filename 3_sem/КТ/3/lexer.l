%{
#include "y.tab.h"
%}

%%

[ \t\n]             
\"([^\"\\]|\\.)*\"   { return STRING; }
[0-9]+               { return NUMBER; }
true|false           { return BOOLEAN; }
:                    { return COLON; }
,                    { return COMMA; }
\{                   { return LEFT_BRACE; }
\}                   { return RIGHT_BRACE; }
\[                   { return LEFT_BRACKET; }
\]                   { return RIGHT_BRACKET; }
\-                   { return MINUS; }  
\+                   { return PLUS; }  
\/                   { return SLASH; }
\*                   { return ASTERISK; }
.                    { yyerror("invalid character"); }

%%

int yywrap() {
    return 1;
}
