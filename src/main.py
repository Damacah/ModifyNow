from translations import translations
import language

from customtkinter import filedialog
import customtkinter

import zipfile

import shutil
import os

current_language = None # Initizalize the variable 

mods_path = os.path.join(os.getenv("APPDATA"), ".minecraft", "mods")

def select_mods(delete_old: bool, status_label):
    
    status_label.configure(text=translations[current_language]["waiting"])

    files = filedialog.askopenfilenames()

    if not files:
        return
    
    if not os.path.exists(mods_path):
        os.makedirs(mods_path)

    if delete_old:
        clear_folder()

    for file in files:

        if file.endswith("zip"):
            print(f"This is a compressed archive: {file}")
            install_compressed(file)
        elif file.endswith("jar"):
            install_jar(file)
        else:
            status_label.configure(text=translations[current_language]["error"])
            return
            
    
    status_label.configure(text=translations[current_language]["completed"])

def install_compressed(file: str):

    with zipfile.ZipFile(file, 'r') as archive:

        archive.extractall(mods_path)

def install_jar(file: str):

    shutil.copy2(file, mods_path)

def clear_folder():

    for filename in os.listdir(mods_path):
        file_path = os.path.join(mods_path, filename)

        try:
            if os.path.isfile(file_path) or os.path.islink(file_path):
                os.unlink(file_path)
            elif os.path.isdir(file_path):
                shutil.rmtree(file_path)

        except Exception as e:
            print(f'Failed to delete {file_path}. Reason: {e}')

def change_language(status_label, install_button, delete_mods_switch, language_button):
    global current_language

    new_language = language.change()

    # Update the UI
    translations_dict = translations[new_language]

    if status_label.cget("text") == translations[current_language]["error"]:
        status_label.configure(text=translations_dict["error"])
    elif status_label.cget("text") == translations[current_language]["completed"]:
        status_label.configure(text=translations_dict["completed"])
    else:
        status_label.configure(text=translations_dict["waiting"])
    install_button.configure(text=translations_dict["select"])
    delete_mods_switch.configure(text=translations_dict["delete_old"])
    language_button.configure(text=translations_dict["change_language"])

    current_language = new_language

def main():
    translations_dict = translations[current_language]
    
    app = customtkinter.CTk()
    app.geometry("400x200")
    app.resizable(False, False)
    app.title("ModifyNow")
    
    main_frame = customtkinter.CTkFrame(app, corner_radius=10)
    main_frame.pack(padx=0, pady=0, expand=True)

    status_label = customtkinter.CTkLabel(main_frame, text=translations_dict["waiting"])
    install_button = customtkinter.CTkButton(main_frame, text=translations_dict["select"], command=lambda: select_mods(delete_mods.get(), status_label))
    
    delete_mods = customtkinter.BooleanVar(value=True)
    delete_mods_switch = customtkinter.CTkSwitch(main_frame, text=translations_dict["delete_old"], variable=delete_mods, onvalue=True, offvalue=False)
    
    language_button = customtkinter.CTkButton(app, text=translations_dict["change_language"], width=60, command=lambda: change_language(status_label, install_button, delete_mods_switch, language_button))

    status_label.pack(padx=20, pady=10)
    install_button.pack(padx=20, pady=10)
    delete_mods_switch.pack(padx=20, pady=10)
    language_button.place(relx=1, rely=1, anchor="se", x=-15, y=-15)


    app.mainloop()


if __name__ == "__main__":    
    current_language = language.startup_check()
    main()