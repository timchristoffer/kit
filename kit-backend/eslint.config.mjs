import { Linter } from "eslint";
import globals from "globals";
import pluginJs from "@eslint/js";
import tseslint from "typescript-eslint";

export default {
    overrides: [
        {
            files: ["**/*.{js,mjs,cjs,ts,tsx}"],
            parserOptions: {
                ecmaVersion: 2021,
                sourceType: "module",
            },
            languageOptions: { globals: globals.browser },
            rules: {
                "no-console": "warn",
            },
        },
    ],
    plugins: [pluginJs, tseslint],
    extends: [
        pluginJs.configs.recommended,
        ...tseslint.configs.recommended,
    ],
};
