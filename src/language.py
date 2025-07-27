import os

language_file = os.path.join(os.getenv("APPDATA"), "ModifyNow", "language.dat")

def startup_check() -> str:

    if not os.path.exists(language_file):
        os.makedirs(os.path.dirname(language_file), exist_ok=True)        
        with open(language_file, 'w', encoding='utf-8') as f:
            f.write("en")

            current_language = "en"

    else:
        with open(language_file, 'r', encoding='utf-8') as f:
            current_language = f.read().strip()

    return current_language

def change() -> str:

    with open(language_file, 'r', encoding='utf-8') as f:
        current_language = f.read().strip()
    
    if current_language == "en":
        new_language = "es"
    else:
        new_language = "en"
    
    with open(language_file, 'w', encoding='utf-8') as f:
        f.write(new_language)

    return new_language #This will be used later as the new "current language"