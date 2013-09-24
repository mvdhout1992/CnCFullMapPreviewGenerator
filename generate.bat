for %%A in (*.ini) do (
call CnCFullMapPreviewGenerator.exe "%%A" --drawvisibleonly
)