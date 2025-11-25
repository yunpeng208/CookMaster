import {ApplicationResponse} from './application-response';

export type ListResponse<T> = ApplicationResponse<{
  Objects: T[];
}>;
