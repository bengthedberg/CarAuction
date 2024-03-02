export { default } from "next-auth/middleware";

export const config = {
  matcher: ["/session"],
  pages: {
    signIn: "/api/auth/signin",  // page that will be displayed when unauthorize.
  },
};
