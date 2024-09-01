import type { Config } from "tailwindcss";

const config: Config = {
  content: [
    "./pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./components/**/*.{js,ts,jsx,tsx,mdx}",
    "./app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {
      colors: {
        'dark-purple': '#1c1b29',
        'neon-light-purple': '#9b5de5',
        'neon-light-blue': '#00f5d4',
      },
      boxShadow: {
        neon: '0 0 15px 3px rgba(155, 93, 229, 0.8), 0 0 15px 3px rgba(0, 245, 212, 0.8)',
      },
    },
  },
  plugins: [],
};
export default config;
