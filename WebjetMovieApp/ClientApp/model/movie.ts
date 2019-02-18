export interface Movie {
    id: string;
    title: string;
    year: string;
    type: string;
    poster: string;
    detail: MovieDetail;
}


export interface MovieDetail {
    rated: string;
    released: string;
    runtime: string;
    director: string;
    writer: string;
    actors: string;
    plot: string;
    language: string;
    country: string;
    awards: string;
    metascore: string;
    rating: string;
    votes: string;
    priceDetail: MoviePrice[];
}
export interface MoviePrice {
    provider: string;
    price: string;
}