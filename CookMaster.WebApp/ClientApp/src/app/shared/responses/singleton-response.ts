import {ApplicationResponse} from "./application-response";

export type SingletonResponse<T> = ApplicationResponse<{
  Object: T;
}>;
