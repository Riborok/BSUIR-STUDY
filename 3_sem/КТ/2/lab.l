%{
#include <stdio.h>
%}

%%
^[ \t]+ 
%%

int main() {
    yylex();
    return 0;
}