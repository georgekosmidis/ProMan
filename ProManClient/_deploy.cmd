@echo off


set development="ProManClient"
set deploy="_Deploy"

xcopy %development%\bin\*.dll %deploy%\bin\ /s /y

xcopy %development%\*.cshtml %deploy%\ /s /y
xcopy %development%\*.resx %deploy%\ /s /y
xcopy %development%\*.edmx %deploy%\ /s /y

xcopy %development%\*.config %deploy%\ /s /y

xcopy %development%\*.xml %deploy%\ /s /y
xcopy %development%\*.xslt %deploy%\ /s /y
xcopy %development%\*.css %deploy%\ /s /y
xcopy %development%\*.js %deploy%\ /s /y
xcopy %development%\*.map %deploy%\ /s /y

xcopy %development%\*.ico %deploy%\ /s /y
xcopy %development%\*.png %deploy%\ /s /y
xcopy %development%\*.jpg %deploy%\ /s /y
xcopy %development%\*.gif %deploy%\ /s /y
xcopy %development%\*.svg %deploy%\ /s /y

xcopy %development%\fonts\*.* %deploy%\fonts\*.* /s /y

xcopy %development%\bin\*.dll %deploy%\bin\ /s /y

xcopy %development%\Global.asax %deploy%\ /s /y
@pause

if errorlevel 1 GOTO ERROR
goto END

:ERROR
echo !!! ERROR !!!
@pause

:END
echo Ended successfully!!!

