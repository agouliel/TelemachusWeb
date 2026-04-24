/* eslint-env node */
module.exports = {
  env: { browser: true, es2022: true, node: false },
  extends: [
    'airbnb',
    'airbnb/hooks',
    'eslint:recommended',
    'plugin:react/recommended',
    'plugin:react-hooks/recommended',
    'plugin:react/jsx-runtime',
    'plugin:import/recommended',
    'plugin:prettier/recommended',
    'prettier'
  ],
  parserOptions: {
    source: 'module',
    sourceType: 'module',
    ecmaVersion: 'latest',
    ecmaFeatures: {
      jsx: true
    },
    requireConfigFile: false
  },
  settings: {
    'import/extensions': ['.js', '.jsx'],
    'import/resolver': {
      node: {
        paths: ['./'],
        extensions: ['.js', '.jsx']
      }
    },
    react: {
      version: 'detect'
    }
  },
  plugins: ['react-hooks', 'prettier', 'import', 'unused-imports'],
  rules: {
    'unused-imports/no-unused-imports': 'error',
    'max-len': [
      'error',
      {
        code: 100,
        tabWidth: 2,
        ignorePattern: '^import',
        ignoreUrls: true,
        ignoreStrings: true,
        ignoreTemplateLiterals: true,
        ignoreComments: true
      }
    ],
    'prettier/prettier': [
      'error',
      {
        endOfLine: 'auto'
      }
    ],
    'react/function-component-definition': [
      2,
      {
        namedComponents: 'arrow-function',
        unnamedComponents: 'arrow-function'
      }
    ],
    'react-hooks/rules-of-hooks': 'error',
    'sort-imports': [
      'error',
      {
        ignoreCase: true,
        ignoreDeclarationSort: true
      }
    ],
    'unused-imports/no-unused-vars': 'off',
    'no-unused-vars': [
      'warn',
      { vars: 'all', varsIgnorePattern: '^_', args: 'after-used', argsIgnorePattern: '^_' }
    ],
    'react/prop-types': 'off',
    'react/require-default-props': 'error',
    'react/jsx-uses-react': 'off',
    'react/react-in-jsx-scope': 'off',
    'consistent-return': 'off',
    'no-shadow': 'off',
    'react/jsx-props-no-spreading': 'off',
    'jsx-a11y/no-static-element-interactions': 'warn',
    'jsx-a11y/click-events-have-key-events': 'warn',
    'import/no-extraneous-dependencies': ['error', { devDependencies: true }],
    'import/prefer-default-export': 'warn',
    'react-hooks/exhaustive-deps': 'off',
    '@tanstack/query/prefer-query-object-syntax': 'off',
    'no-restricted-syntax': ['off', 'ForOfStatement'],
    'import/no-unresolved': 'error',
    'import/named': 'error',
    'import/namespace': 'error',
    'import/default': 'error',
    'import/export': 'error',
    'import/no-named-as-default': 'warn',
    'no-continue': 'off',
    'react/jsx-filename-extension': 'error',
    'no-param-reassign': 'off',
    'no-nested-ternary': 'off',
    'no-console': 'off',
    'jsx-a11y/no-autofocus': 'off',
    'no-unreachable': 'warn'
  }
}
