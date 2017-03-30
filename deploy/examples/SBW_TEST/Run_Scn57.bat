
set scn=57

set /p=Running Scenario %scn%. Press ENTER to continue...
set drive=C:
set workingdir=C:\BRM\LANDIS_II\Code\GitCode\Extension-SpruceBudworm\deploy\examples\SBW_TEST

if not exist %workingdir%\Scn%scn% mkdir %workingdir%\Scn%scn%
%drive%
cd %workingdir%\Scn%scn%
copy %workingdir%\Scenario_SBW_Test_Scn%scn%.txt
call landis Scenario_SBW_Test_Scn%scn%.txt
%drive%
cd %workingdir%\Scn%scn%
