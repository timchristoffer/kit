import { Montserrat, Space_Grotesk, Lato} from 'next/font/google';

// // Font för rubriker
// export const poppins = Poppins({
//   subsets: ['latin'],
//   display: 'swap',
//   weight: ['500', '600', '700'],
// });

// // Font för brödtext och UI-element
// export const inter = Inter({
//   subsets: ['latin'],
//   display: 'swap',
//   weight: ['400', '500', '600'],
// });

export const montserrat = Montserrat({
  subsets: ['latin'],
    display: 'swap',
    weight: ['400', '500', '600'],
  });


export const space_grotesk = Space_Grotesk({
  subsets: ['latin'],
display: 'swap',
  weight: ['500', '700'],
  variable: '--font-title',
});

export const lato = Lato({
  subsets: ['latin'],
  display: 'swap',
   weight: ['400', '700'],
   variable: '--font-body',
 });